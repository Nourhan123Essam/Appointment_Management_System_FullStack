using Appointment_System.Application.Features.Doctor.Commands;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Interfaces;
using Moq;
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Entities;
using Appointment_System.Application.Features.Doctor.Queries;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.ValueObjects;
using Microsoft.Extensions.Localization;
using Appointment_System.Application.Localization;

namespace Appointment_System.Application.Tests
{
    public class DoctorServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IIdentityRepository> _identityMock;
        private Mock<IRedisCacheService> _redisMock;
        private Mock<ISpecializationRepository> _specializationRepoMock;
        private Mock<IDoctorRepository> _doctorRepoMock;
        private Mock<IOfficeRepository> _officeRepoMock;
        private Mock<ILocalizationService> _localizationServiceMock;

        private CreateDoctorCommandHandler _handler;
        private UpdateDoctorHandler _handler_UpdateBasicData;
        private UpdateDoctorTranslationCommandHandler _handler_UpdateDoctorTranslation;
        private DeleteDoctorHandler _handler_DeleteDoctor;
        private GetDoctorByIdQueryHandler _handler_GetDoctorByIdQueryHandler;
        private GetAdminDoctorsQueryHandler _handler_GetAdminDoctorsQueryHandler;
        private GetPublicDoctorsQueryHandler _handler_GetPublicDoctorsQueryHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _identityMock = new Mock<IIdentityRepository>();
            _redisMock = new Mock<IRedisCacheService>();
            _localizationServiceMock = new Mock<ILocalizationService>();

            _specializationRepoMock = new Mock<ISpecializationRepository>();
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _officeRepoMock = new Mock<IOfficeRepository>();

            _unitOfWorkMock.Setup(x => x.SpecializationRepository)
                .Returns(_specializationRepoMock.Object);

            _unitOfWorkMock.Setup(x => x.DoctorRepository)
                .Returns(_doctorRepoMock.Object);

            _unitOfWorkMock.Setup(x => x.OfficeRepository)
                .Returns(_officeRepoMock.Object);

            _handler = new CreateDoctorCommandHandler(_unitOfWorkMock.Object, _identityMock.Object, _redisMock.Object);
            _handler_UpdateBasicData = new UpdateDoctorHandler(_unitOfWorkMock.Object, _redisMock.Object);
            _handler_UpdateDoctorTranslation = new UpdateDoctorTranslationCommandHandler(_unitOfWorkMock.Object, _redisMock.Object);
            _handler_DeleteDoctor = new DeleteDoctorHandler(_unitOfWorkMock.Object, _redisMock.Object);
            _handler_GetDoctorByIdQueryHandler = new GetDoctorByIdQueryHandler(_unitOfWorkMock.Object, _redisMock.Object);
            _handler_GetAdminDoctorsQueryHandler = new GetAdminDoctorsQueryHandler(
                _redisMock.Object, _unitOfWorkMock.Object,
                _specializationRepoMock.Object, _localizationServiceMock.Object);
            _handler_GetPublicDoctorsQueryHandler = new GetPublicDoctorsQueryHandler(
                _redisMock.Object, _unitOfWorkMock.Object,
                _specializationRepoMock.Object, _localizationServiceMock.Object);
        }


        #region Create Doctor

        [Test]
        public async Task Handle_Should_ReturnFailure_When_EmailAlreadyExists()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "exists@example.com",
                Phone = "123456789",
                Password = password,
                ImageUrl = "image.jpg",
                InitialFee = 500,
                FollowUpFee = 250,
                MaxFollowUps = 3,
                YearsOfExperience = 10,
                SpecializationIds = new List<int> { 1 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "John", LastName = "Smith", Language = "en", Bio = "Bio text" }
                },
                Qualifications = new(),
                Availabilities = new()
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(true); // Email already in use

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("A user with this email already exists."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_SpecializationIdDoesNotExist()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "rolefail@example.com",
                Phone = "123456789",
                Password = password,
                ImageUrl = "image.jpg",
                InitialFee = 500,
                FollowUpFee = 250,
                MaxFollowUps = 3,
                YearsOfExperience = 10,

                SpecializationIds = new List<int> { 1, 2 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "John", LastName = "Doe", Language = "en", Bio = "Some bio" }
                },
                Qualifications = new List<CreateQualificationDto>(),
                Availabilities = new List<CreateDoctorAvailabilityDto>()
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(false);

            _identityMock
                .Setup(x => x.CreateUserAsync(dto.Email, password))
                .ReturnsAsync("new-user-id");

            _identityMock
                .Setup(x => x.AssignRoleAsync("new-user-id", "Doctor"))
                .ReturnsAsync(true);

            _specializationRepoMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _specializationRepoMock
                .Setup(x => x.ExistsAsync(2))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Specialization ID 2 does not exist."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_OfficeIdDoesNotExist()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "officefail@example.com",
                Phone = "123456789",
                Password = password,
                ImageUrl = "image.jpg",
                InitialFee = 500,
                FollowUpFee = 250,
                MaxFollowUps = 3,
                YearsOfExperience = 10,

                SpecializationIds = new List<int> { 1 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "Jane", LastName = "Doe", Language = "en", Bio = "Experienced doctor" }
                },
                Qualifications = new List<CreateQualificationDto>(),
                Availabilities = new List<CreateDoctorAvailabilityDto>
                {
                    new() { 
                        DayOfWeek = DayOfWeek.Monday, 
                        StartTime = new TimeSpan(9, 0, 0), 
                        EndTime = new TimeSpan(17, 0, 0),
                        OfficeId = 99 // ❌ invalid office ID
                    }
                }
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(false);

            _specializationRepoMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _officeRepoMock
                .Setup(x => x.ExistsAsync(99))
                .ReturnsAsync(false); // Force failure on office ID

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Office ID 99 does not exist."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_CreateUserFails()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "rolefail@example.com",
                Phone = "123456789",
                Password = password,
                ImageUrl = "image.jpg",
                InitialFee = 500,
                FollowUpFee = 250,
                MaxFollowUps = 3,
                YearsOfExperience = 10,

                SpecializationIds = new List<int> { 1 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "Jane", LastName = "Doe", Language = "en", Bio = "Bio" }
                },
                Qualifications = new List<CreateQualificationDto>(),
                Availabilities = new List<CreateDoctorAvailabilityDto>()
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(false);

            _specializationRepoMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _doctorRepoMock
                .Setup(x => x.CreateDoctorWithUserAsync(dto, password))
                .ReturnsAsync(Result<Doctor>.Fail("Failed to create user.")); // Simulate failure

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to create user."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_AssignRoleFails()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "rolefail@example.com",
                Phone = "123456789",
                Password = password,
                ImageUrl = "image.jpg",
                InitialFee = 500,
                FollowUpFee = 250,
                MaxFollowUps = 3,
                YearsOfExperience = 10,
                SpecializationIds = new List<int> { 1 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "Jane", LastName = "Doe", Language = "en", Bio = "Bio" }
                },
                Qualifications = new List<CreateQualificationDto>(),
                Availabilities = new List<CreateDoctorAvailabilityDto>()
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(false);

            _identityMock
                .Setup(x => x.CreateUserAsync(dto.Email, password))
                .ReturnsAsync("new-user-id");

            _identityMock
                .Setup(x => x.AssignRoleAsync("new-user-id", "Doctor"))
                .ReturnsAsync(false); // Simulate role assignment failure

            _unitOfWorkMock
                .Setup(x => x.SpecializationRepository.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _doctorRepoMock
                .Setup(x => x.CreateDoctorWithUserAsync(dto, password))
                .ReturnsAsync(Result<Doctor>.Fail("Failed to assign Doctor role."));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to assign Doctor role."));
        }

        [Test]
        public async Task Handle_Should_CreateDoctor_And_AddToRedis_When_NoCacheExists()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "doctor@example.com",
                Phone = "01000000000",
                Password = password,
                ImageUrl = "image.jpg",
                InitialFee = 500,
                FollowUpFee = 300,
                MaxFollowUps = 3,
                YearsOfExperience = 10,
                SpecializationIds = new List<int> { 1 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "Ali", LastName = "Youssef", Language = "en-US", Bio = "Bio" }
                },
                Qualifications = new List<CreateQualificationDto>(),
                Availabilities = new List<CreateDoctorAvailabilityDto>()
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(false);

            _identityMock
                .Setup(x => x.CreateUserAsync(dto.Email, password))
                .ReturnsAsync("user-id");

            _identityMock
                .Setup(x => x.AssignRoleAsync("user-id", "Doctor"))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(x => x.SpecializationRepository.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _redisMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync((List<DoctorBasicDto>?)null);

            var doctor = dto.ToEntity("user-id");
            _doctorRepoMock
                .Setup(x => x.CreateDoctorWithUserAsync(dto, password))
                .ReturnsAsync(Result<Doctor>.Success(doctor));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor created successfully."));

            _redisMock.Verify(x => x.SetAllDoctorsAsync(
                It.Is<List<DoctorBasicDto>>(list =>
                    list.Count == 1 && list[0].Email == dto.Email
                )
            ), Times.Once);
        }

        [Test]
        public async Task Handle_Should_CreateDoctorSuccessfully_When_AllDataIsValid()
        {
            // Arrange
            string password = "Secure123!";
            var dto = new CreateDoctorDto
            {
                Email = "happy@example.com",
                Phone = "1234567890",
                ImageUrl = "url",
                InitialFee = 200,
                FollowUpFee = 100,
                MaxFollowUps = 3,
                YearsOfExperience = 10,
                Password = password,
                SpecializationIds = new List<int> { 1 },
                Translations = new List<DoctorTranslationDto>
                {
                    new() { FirstName = "Happy", LastName = "Doctor", Bio = "Bio", Language = "en-US" }
                },
                Qualifications = new List<CreateQualificationDto>
                {
                    new()
                    {
                        Translations = new List<QualificationTranslationDto>
                        {
                            new() { Title = "MD", Institution = "Harvard", Language = "en-US" }
                        },
                        Date = 2015
                    }
                },
                Availabilities = new List<CreateDoctorAvailabilityDto>
                {
                    new()
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        OfficeId = 1
                    }
                }
            };

            var command = new CreateDoctorCommand(dto, password);

            _identityMock
                .Setup(x => x.UserExistsAsync(dto.Email))
                .ReturnsAsync(false);

            _specializationRepoMock
                .Setup(x => x.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _officeRepoMock
                .Setup(x => x.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var createdDoctor = dto.ToEntity("user-123");

            _doctorRepoMock
                .Setup(x => x.CreateDoctorWithUserAsync(dto, password))
                .ReturnsAsync(Result<Doctor>.Success(createdDoctor));

            var existingDoctors = new List<DoctorBasicDto>
            {
                new() { Email = "existing@example.com" }
            };

            _redisMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync(existingDoctors);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor created successfully."));

            _redisMock.Verify(x => x.SetAllDoctorsAsync(
                It.Is<List<DoctorBasicDto>>(list =>
                    list.Count == 2 &&
                    list.Any(d => d.Email == "happy@example.com") &&
                    list.Any(d => d.Email == "existing@example.com")
                )
            ), Times.Once);
        }


        #endregion

        #region Update Basic Doctor data
        [Test]
        public async Task Handle_Should_ReturnFailure_When_DtoIsNull()
        {
            // Arrange
            var command = new UpdateDoctorCommand(null); // null DTO

            // Act
            var result = await _handler_UpdateBasicData.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid request."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_SpecializationIdIsInvalid()
        {
            // Arrange
           var dto = new UpdateDoctorDto
            {
                Id = 1,
                SpecializationIds = new List<int> { 1, 2 }, // Let's say 2 is invalid
                Phone = "123456789",
                ImageUrl = "test.jpg",
                InitialFee = 100,
                FollowUpFee = 50,
                MaxFollowUps = 3,
                YearsOfExperience = 10
            };

            var command = new UpdateDoctorCommand(dto);

            var doctor = new Doctor
            {
                Id = dto.Id,
                DoctorSpecializations = new List<DoctorSpecialization>(),
                Phone = "OldPhone",
                ImageUrl = "OldImage",
                InitialFee = 0,
                FollowUpFee = 0,
                MaxFollowUps = 0,
                YearsOfExperience = 0
            };

            // Setup: Specialization 1 exists, 2 does not
            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.GetDoctorByIdAsync(dto.Id))
                .ReturnsAsync(doctor);
            _unitOfWorkMock.Setup(x => x.SpecializationRepository.ExistsAsync(1)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.SpecializationRepository.ExistsAsync(2)).ReturnsAsync(false);

            // Act
            var result = await _handler_UpdateBasicData.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Specialization ID 2 does not exist."));
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_DataIsValid()
        {
            // Arrange
            var dto = new UpdateDoctorDto
            {
                Id = 1,
                SpecializationIds = new List<int> { 1, 2 },
                Phone = "123456789",
                ImageUrl = "image.jpg",
                InitialFee = 200,
                FollowUpFee = 100,
                MaxFollowUps = 5,
                YearsOfExperience = 12
            };

            var command = new UpdateDoctorCommand(dto);

            var existingDoctor = new Doctor
            {
                Id = dto.Id,
                UserId = "user-123",
                DoctorSpecializations = new List<DoctorSpecialization>(),
                Phone = "old",
                ImageUrl = "old.jpg",
                InitialFee = 0,
                FollowUpFee = 0,
                MaxFollowUps = 0,
                YearsOfExperience = 0
            };

            // Setup all specialization IDs as valid
            _unitOfWorkMock.Setup(x => x.SpecializationRepository.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

            // Setup: doctor exists
            _unitOfWorkMock.Setup(x => x.DoctorRepository.GetDoctorByIdAsync(dto.Id)).ReturnsAsync(existingDoctor);

            // Setup: save changes returns success
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler_UpdateBasicData.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor updated successfully."));

            // Verify doctor update
            _unitOfWorkMock.Verify(x => x.DoctorRepository.UpdateBasicAsync(It.Is<Doctor>(d =>
                d.Phone == dto.Phone &&
                d.ImageUrl == dto.ImageUrl &&
                d.InitialFee == dto.InitialFee &&
                d.FollowUpFee == dto.FollowUpFee &&
                d.MaxFollowUps == dto.MaxFollowUps &&
                d.YearsOfExperience == dto.YearsOfExperience
            )), Times.Once);

            // Verify changes saved
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_Should_ReturnFail_When_DoctorNotFound()
        {
            // Arrange
            var dto = new UpdateDoctorDto
            {
                Id = 99, // Non-existent ID
                SpecializationIds = new List<int> { 1, 2 },
                Phone = "123456789",
                ImageUrl = "image.jpg",
                InitialFee = 200,
                FollowUpFee = 100,
                MaxFollowUps = 5,
                YearsOfExperience = 12
            };

            var command = new UpdateDoctorCommand(dto);

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.GetDoctorByIdAsync(dto.Id))
                .ReturnsAsync((Doctor?)null);

            // Act
            var result = await _handler_UpdateBasicData.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Doctor not found."));

            _unitOfWorkMock.Verify(x => x.DoctorRepository.UpdateBasicAsync(It.IsAny<Doctor>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task Handle_Should_UpdateCachedDoctor_When_CacheExists()
        {
            // Arrange
            var dto = new UpdateDoctorDto
            {
                Id = 1,
                SpecializationIds = new List<int> { 1 },
                Phone = "987654321",
                ImageUrl = "updated.jpg",
                InitialFee = 300,
                FollowUpFee = 150,
                MaxFollowUps = 10,
                YearsOfExperience = 15
            };

            var command = new UpdateDoctorCommand(dto);

            var existingDoctor = new Doctor
            {
                Id = dto.Id,
                UserId = "user-1",
                DoctorSpecializations = new List<DoctorSpecialization>(),
                Phone = "old",
                ImageUrl = "old.jpg",
                InitialFee = 0,
                FollowUpFee = 0,
                MaxFollowUps = 0,
                YearsOfExperience = 0
            };

            var cachedDoctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = dto.Id,
                    Phone = "old",
                    ImageUrl = "old.jpg",
                    InitialFee = 0,
                    FollowUpFee = 0,
                    MaxFollowUps = 0,
                    YearsOfExperience = 0
                }
            };

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.GetDoctorByIdAsync(dto.Id))
                .ReturnsAsync(existingDoctor);

            _unitOfWorkMock
                .Setup(x => x.SpecializationRepository.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _redisMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync(cachedDoctors);

            // Act
            var result = await _handler_UpdateBasicData.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor updated successfully."));

            _redisMock.Verify(x => x.SetAllDoctorsAsync(It.Is<List<DoctorBasicDto>>(list =>
                list.Count == 1 &&
                list[0].Id == dto.Id &&
                list[0].Phone == dto.Phone &&
                list[0].ImageUrl == dto.ImageUrl &&
                list[0].InitialFee == dto.InitialFee &&
                list[0].FollowUpFee == dto.FollowUpFee &&
                list[0].MaxFollowUps == dto.MaxFollowUps &&
                list[0].YearsOfExperience == dto.YearsOfExperience
            )), Times.Once);
        }

        #endregion

        #region Update Doctor Translation
        [Test]
        public async Task Handle_Should_ReturnFailure_When_DoctorDoesNotExist()
        {
            // Arrange
            var translationDto = new DoctorTranslationDto
            {
                Id = 1,
                DoctorId = 99, // Non-existing doctor
                Language = "en-US",
                FirstName = "John",
                LastName = "Doe",
                Bio = "Bio here"
            };

            var command = new UpdateDoctorTranslationCommand(translationDto);

            // Setup mock to return null for non-existing doctor
            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorByIdAsync(translationDto.DoctorId))
                .ReturnsAsync((Doctor)null);

            // Act
            var result = await _handler_UpdateDoctorTranslation.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Doctor not found."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_TranslationDoesNotExist()
        {
            // Arrange
            var translationDto = new DoctorTranslationDto
            {
                Id = 10, // Non-existing translation ID
                DoctorId = 1, // Existing doctor ID
                Language = "en-US",
                FirstName = "John",
                LastName = "Doe",
                Bio = "Bio here"
            };

            var command = new UpdateDoctorTranslationCommand(translationDto);

            // Setup mock to return a valid doctor
            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorByIdAsync(translationDto.DoctorId))
                .ReturnsAsync(new Doctor { Id = translationDto.DoctorId });

            // Setup mock to return null for translation not found
            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorTranslationByIdAsync(translationDto.Id))
                .ReturnsAsync((DoctorTranslation)null);

            // Act
            var result = await _handler_UpdateDoctorTranslation.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Doctor translation not found."));
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_TranslationUpdatedSuccessfully()
        {
            // Arrange
            var translationDto = new DoctorTranslationDto
            {
                Id = 5,
                DoctorId = 1,
                Language = "en-US",
                FirstName = "John",
                LastName = "Doe",
                Bio = "Updated bio"
            };

            var command = new UpdateDoctorTranslationCommand(translationDto);

            var existingDoctor = new Doctor { Id = translationDto.DoctorId };
            var existingTranslation = new DoctorTranslation
            {
                Id = translationDto.Id,
                DoctorId = translationDto.DoctorId,
                Language = new Domain.ValueObjects.Language("en-US"),
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                Bio = "Old bio"
            };

            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorByIdAsync(translationDto.DoctorId))
                .ReturnsAsync(existingDoctor);

            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorTranslationByIdAsync(translationDto.Id))
                .ReturnsAsync(existingTranslation);

            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.UpdateTranslationAsync(It.IsAny<DoctorTranslation>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler_UpdateDoctorTranslation.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor translation updated successfully."));

            // Verify that the UpdateAsync method was called with the updated entity
            _unitOfWorkMock.Verify(u => u.DoctorRepository.UpdateTranslationAsync(
                It.Is<DoctorTranslation>(t =>
                    t.Id == translationDto.Id &&
                    t.FirstName == translationDto.FirstName &&
                    t.LastName == translationDto.LastName &&
                    t.Bio == translationDto.Bio
                )), Times.Once);

            // Verify SaveChangesAsync was called
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_Should_UpdateCache_When_TranslationExistsInCache()
        {
            // Arrange
            var translationDto = new DoctorTranslationDto
            {
                Id = 5,
                DoctorId = 1,
                Language = "en-US",
                FirstName = "John",
                LastName = "Doe",
                Bio = "Updated bio"
            };

            var command = new UpdateDoctorTranslationCommand(translationDto);

            var existingDoctor = new Doctor { Id = translationDto.DoctorId };
            var existingTranslation = new DoctorTranslation
            {
                Id = translationDto.Id,
                DoctorId = translationDto.DoctorId,
                Language = new Domain.ValueObjects.Language("en-US"),
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                Bio = "Old bio"
            };

            var cachedDoctor = new DoctorBasicDto
            {
                Id = 1,
                FirstNames = new Dictionary<string, string> { { "en-US", "OldFirstName" } },
                LastNames = new Dictionary<string, string> { { "en-US", "OldLastName" } },
                Bio = new Dictionary<string, string> { { "en-US", "Old bio" } }
            };

            var cachedDoctorsList = new List<DoctorBasicDto> { cachedDoctor };

            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorByIdAsync(translationDto.DoctorId))
                .ReturnsAsync(existingDoctor);

            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.GetDoctorTranslationByIdAsync(translationDto.Id))
                .ReturnsAsync(existingTranslation);

            _unitOfWorkMock
                .Setup(u => u.DoctorRepository.UpdateTranslationAsync(It.IsAny<DoctorTranslation>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _redisMock
                .Setup(r => r.GetAllDoctorsAsync())
                .ReturnsAsync(cachedDoctorsList);

            _redisMock
                .Setup(r => r.SetAllDoctorsAsync(It.IsAny<List<DoctorBasicDto>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler_UpdateDoctorTranslation.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor translation updated successfully."));

            Assert.That(cachedDoctor.FirstNames["en-US"], Is.EqualTo("John"));
            Assert.That(cachedDoctor.LastNames["en-US"], Is.EqualTo("Doe"));
            Assert.That(cachedDoctor.Bio["en-US"], Is.EqualTo("Updated bio"));

            _redisMock.Verify(r => r.SetAllDoctorsAsync(It.Is<List<DoctorBasicDto>>(list =>
                list.Any(d =>
                    d.Id == 1 &&
                    d.FirstNames["en-US"] == "John" &&
                    d.LastNames["en-US"] == "Doe" &&
                    d.Bio["en-US"] == "Updated bio"
                ))), Times.Once);
        }

        #endregion

        #region Delete Doctor
        [Test]
        public async Task Handle_Should_ReturnFailure_When_DoctorDoesNotExist_when_Delete()
        {
            // Arrange
            var command = new DeleteDoctorCommand(doctorId: 10);

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.GetDoctorByIdAsync(10))
                .ReturnsAsync((Doctor)null);

            // Act
            var result = await _handler_DeleteDoctor.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Doctor not found."));
        }

        [Test]
        public async Task Handle_Should_DeleteDoctor_When_DoctorExists()
        {
            // Arrange
            var command = new DeleteDoctorCommand(doctorId: 5);

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.GetDoctorByIdAsync(5))
                .ReturnsAsync(new Doctor { Id = 5 });

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.DeleteAsync(5))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler_DeleteDoctor.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor deleted."));

            _unitOfWorkMock.Verify(x => x.DoctorRepository.DeleteAsync(5), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Handle_Should_RemoveDoctorFromRedisCache_When_CacheExists()
        {
            // Arrange
            var command = new DeleteDoctorCommand(doctorId: 5);

            var cachedDoctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto { Id = 5, Email = "toremove@example.com" },
                new DoctorBasicDto { Id = 6, Email = "tokeep@example.com" }
            };

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.GetDoctorByIdAsync(5))
                .ReturnsAsync(new Doctor { Id = 5 });

            _unitOfWorkMock
                .Setup(x => x.DoctorRepository.DeleteAsync(5))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _redisMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync(cachedDoctors);

            // Act
            var result = await _handler_DeleteDoctor.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Doctor deleted."));

            _redisMock.Verify(x => x.SetAllDoctorsAsync(
                It.Is<List<DoctorBasicDto>>(list =>
                    list.Count == 1 &&
                    list[0].Id == 6 &&
                    list[0].Email == "tokeep@example.com"
                )
            ), Times.Once);
        }


        #endregion


        // ============= Fetching handlers ===============

        #region Get By Id Query
        [Test]
        public async Task Handle_ShouldReturnSuccessResult_WhenDoctorExistsWithTranslation()
        {
            // Arrange
            var doctorId = 1;
            var language = "en-US";

            var doctorInCache = new DoctorBasicDto
            {
                Id = doctorId,
                UserId = "user-123",
                FirstNames = new Dictionary<string, string> { { language, "John" } },
                LastNames = new Dictionary<string, string> { { language, "Doe" } },
                Bio = new Dictionary<string, string> { { language, "Experienced doctor" } },
                TranslationIds = new Dictionary<string, int> { { language, 100 } },
                SpecializationNames = new Dictionary<string, List<string>>
                {
                    { language, new List<string> { "Cardiology", "Internal Medicine" } }
                },
                Phone = "1234567890",
                Email = "john.doe@example.com",
                ImageUrl = "doctor-image.jpg",
                InitialFee = 100,
                FollowUpFee = 50,
                MaxFollowUps = 2,
                YearsOfExperience = 10,
                RatingPoints = 45,
                NumberOfRatings = 10
            };

            _redisMock.Setup(r => r.GetAllDoctorsAsync())
                .ReturnsAsync(new List<DoctorBasicDto> { doctorInCache });

            var query = new GetDoctorByIdQuery(id: doctorId);

            // Act
            var result = await _handler_GetDoctorByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Id, Is.EqualTo(doctorId));
            Assert.That(result.Data.Email, Is.EqualTo("john.doe@example.com"));
            Assert.That(result.Data.Phone, Is.EqualTo("1234567890"));
            Assert.That(result.Data.ImageUrl, Is.EqualTo("doctor-image.jpg"));
            Assert.That(result.Data.InitialFee, Is.EqualTo(100));
            Assert.That(result.Data.FollowUpFee, Is.EqualTo(50));
            Assert.That(result.Data.YearsOfExperience, Is.EqualTo(10));
            Assert.That(result.Data.RatingPoints, Is.EqualTo(45));
            Assert.That(result.Data.NumberOfRatings, Is.EqualTo(10));
            Assert.That(result.Data.Specializations, Is.EquivalentTo(new List<string> { "Cardiology", "Internal Medicine" }));

            // Check translations
            Assert.That(result.Data.Translations, Has.Count.EqualTo(1));
            var translation = result.Data.Translations.First(t => t.Language == language);
            Assert.That(translation.FirstName, Is.EqualTo("John"));
            Assert.That(translation.LastName, Is.EqualTo("Doe"));
            Assert.That(translation.Bio, Is.EqualTo("Experienced doctor"));
        }

        [Test]
        public async Task Handle_ShouldReturnFail_WhenDoctorDoesNotExist()
        {
            // Arrange
            var doctorId = 999; // ID not found in cache

            var otherDoctor = new DoctorBasicDto
            {
                Id = 1,
                FirstNames = new Dictionary<string, string> { { "en", "Alice" } },
                LastNames = new Dictionary<string, string> { { "en", "Smith" } },
                Bio = new Dictionary<string, string> { { "en", "Dermatologist" } },
                SpecializationNames = new Dictionary<string, List<string>>
                {
                    { "en", new List<string> { "Dermatology" } }
                },
                Phone = "1112223333",
                Email = "alice.smith@example.com",
                ImageUrl = "alice.jpg",
                InitialFee = 80,
                FollowUpFee = 40,
                MaxFollowUps = 2,
                YearsOfExperience = 8,
                RatingPoints = 40,
                NumberOfRatings = 10
            };

            _redisMock.Setup(r => r.GetAllDoctorsAsync())
                .ReturnsAsync(new List<DoctorBasicDto> { otherDoctor });

            var query = new GetDoctorByIdQuery(id: doctorId);

            // Act
            var result = await _handler_GetDoctorByIdQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Doctor not found."));
            Assert.That(result.Data, Is.Null);
        }

        #endregion

        #region Get Public Doctors Query
        [Test]
        public async Task Handle_Should_ReturnFail_WhenSpecializationNotFound()
        {
            // Arrange
            var specializationId = 42;
            var language = "en-US";

            var query = new GetPublicDoctorsQuery(new DoctorFilterOptions
            {
                SpecializationId = specializationId,
                Language = language,
                PageNumber = 1,
                PageSize = 10
            });

            _specializationRepoMock
                .Setup(r => r.ExistsAsync(specializationId))
                .ReturnsAsync(false);

            _localizationServiceMock
                .Setup(l => l["SpecializationNotFound"])
                .Returns(new LocalizedString("SpecializationNotFound", "Specialization not found"));

            // Act
            var result = await _handler_GetPublicDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Specialization not found"));
        }

        [Test]
        public async Task Handle_Should_FetchFromCache_IfAvailable()
        {
            // Arrange
            var filterOptions = new DoctorFilterOptions
            {
                Language = "en-US",
                PageNumber = 1,
                PageSize = 2
            };

            var query = new GetPublicDoctorsQuery(filterOptions);

            var cachedDoctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 1,
                    FirstNames = new() { ["en-US"] = "John" },
                    LastNames = new() { ["en-US"] = "Doe" },
                    Bio = new() { ["en-US"] = "Expert in heart surgery" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Cardiology" } },
                    Email = "john@example.com",
                    Phone = "123",
                    ImageUrl = "img1.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                },
                new DoctorBasicDto
                {
                    Id = 2,
                    FirstNames = new() { ["en-US"] = "Jane" },
                    LastNames = new() { ["en-US"] = "Smith" },
                    Bio = new() { ["en-US"] = "Skin specialist" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Dermatology" } },
                    Email = "jane@example.com",
                    Phone = "456",
                    ImageUrl = "img2.jpg",
                    InitialFee = 120,
                    FollowUpFee = 60,
                    YearsOfExperience = 8,
                    RatingPoints = 40,
                    NumberOfRatings = 8
                }
            };

            _redisMock.Setup(c => c.GetAllDoctorsAsync())
                      .ReturnsAsync(cachedDoctors);

            // Act
            var result = await _handler_GetPublicDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Items.Count, Is.EqualTo(2));
            Assert.That(result.Data.Items[0].FirstName, Is.EqualTo("John"));
            Assert.That(result.Data.Items[1].FirstName, Is.EqualTo("Jane"));

            // Ensure DB was NOT called
            _unitOfWorkMock.Verify(u => u.DoctorRepository.GetAllForCacheAsync(), Times.Never);
        }

        [Test]
        public async Task Handle_Should_FetchFromDbAndCacheIfCacheIsNull()
        {
            // Arrange
            var filterOptions = new DoctorFilterOptions
            {
                Language = "en-US",
                PageNumber = 1,
                PageSize = 1
            };

            var query = new GetPublicDoctorsQuery(filterOptions);

            List<DoctorBasicDto> dbDoctors = new()
            {
                new DoctorBasicDto
                {
                    Id = 1,
                    FirstNames = new() { ["en-US"] = "Amina" },
                    LastNames = new() { ["en-US"] = "Hassan" },
                    Bio = new() { ["en-US"] = "General Practitioner" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "General Medicine" } },
                    Email = "amina@example.com",
                    Phone = "789",
                    ImageUrl = "amina.jpg",
                    InitialFee = 80,
                    FollowUpFee = 40,
                    YearsOfExperience = 5,
                    RatingPoints = 30,
                    NumberOfRatings = 5
                }
            };

            _redisMock.Setup(c => c.GetAllDoctorsAsync())
                      .ReturnsAsync((List<DoctorBasicDto>?)null);

            _unitOfWorkMock.Setup(u => u.DoctorRepository.GetAllForCacheAsync())
                           .ReturnsAsync(dbDoctors);

            _redisMock.Setup(c => c.SetAllDoctorsAsync(dbDoctors))
                      .Returns(Task.CompletedTask);

            // Act
            var result = await _handler_GetPublicDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Items.Count, Is.EqualTo(1));
            Assert.That(result.Data.Items[0].FirstName, Is.EqualTo("Amina"));

            // Ensure DB was called and result cached
            _unitOfWorkMock.Verify(u => u.DoctorRepository.GetAllForCacheAsync(), Times.Once);
            _redisMock.Verify(r => r.SetAllDoctorsAsync(dbDoctors), Times.Once);
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_PageNumberExceedsTotalPages()
        {
            // Arrange
            var doctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 1,
                    FirstNames = new() { ["en-US"] = "Sarah" },
                    LastNames = new() { ["en-US"] = "Youssef" },
                    Bio = new() { ["en-US"] = "Dentist" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Dentistry" } },
                    Email = "sarah@example.com",
                    Phone = "555",
                    ImageUrl = "sarah.jpg",
                    InitialFee = 70,
                    FollowUpFee = 35,
                    YearsOfExperience = 4,
                    RatingPoints = 20,
                    NumberOfRatings = 4
                }
            };

            var filterOptions = new DoctorFilterOptions
            {
                Language = "en-US",
                PageNumber = 5, // too high for this dataset
                PageSize = 1
            };

            var query = new GetPublicDoctorsQuery(filterOptions);

            _redisMock.Setup(c => c.GetAllDoctorsAsync())
                      .ReturnsAsync(doctors);

            // Act
            var result = await _handler_GetPublicDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Does.Contain("Page number"));
        }

        #endregion

        #region Get Admin Doctors Query
        [Test]
        public async Task Handle_Should_ReturnFail_WhenSpecializationNotFound_ForAdmin()
        {
            // Arrange
            var specializationId = 99;
            var language = "en-US";

            var query = new GetAdminDoctorsQuery(new DoctorFilterOptions
            {
                SpecializationId = specializationId,
                Language = language,
                PageNumber = 1,
                PageSize = 10
            });

            // Mock: Specialization doesn't exist
            _specializationRepoMock
                .Setup(r => r.ExistsAsync(specializationId))
                .ReturnsAsync(false);

            // Mock: Localizer returns correct string
            _localizationServiceMock
                .Setup(l => l["SpecializationNotFound"])
                .Returns(new LocalizedString("SpecializationNotFound", "Specialization not found"));

            // Act
            var result = await _handler_GetAdminDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Specialization not found"));
        }

        [Test]
        public async Task Handle_Should_ReturnFromCache_WhenDoctorsCached()
        {
            // Arrange
            var language = "en-US";
            var doctorList = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 1,
                        UserId = "user-123",
                    FirstNames = new Dictionary<string, string> { { language, "John" } },
                    LastNames = new Dictionary<string, string> { { language, "Doe" } },
                    Bio = new Dictionary<string, string> { { language, "Experienced doctor" } },
                    TranslationIds = new Dictionary<string, int> { { language, 100 } },
                    SpecializationNames = new Dictionary<string, List<string>>
                    {
                        { language, new List<string> { "Cardiology", "Internal Medicine" } }
                    },
                    Phone = "1234567890",
                    Email = "john.doe@example.com",
                    ImageUrl = "doctor-image.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    MaxFollowUps = 2,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                }
            };

            _redisMock.Setup(c => c.GetAllDoctorsAsync()).ReturnsAsync(doctorList);

            var query = new GetAdminDoctorsQuery(new DoctorFilterOptions
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Act
            var result = await _handler_GetAdminDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Items.Count, Is.EqualTo(1));
            Assert.That(result.Data.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_Should_FetchFromDbAndCache_WhenDoctorsNotCached()
        {
            // Arrange
            var language = "en-US";
            var doctorList = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 2,
                    UserId = "user-123",
                    FirstNames = new Dictionary<string, string> { ["en-US"] = "James", ["ar-EG"] = "جيمس" },
                    LastNames = new Dictionary<string, string> { { language, "Doe" } },
                    Bio = new Dictionary<string, string> { { language, "Experienced doctor" } },
                    TranslationIds = new Dictionary<string, int> { ["en-US"] = 100, ["ar-EG"] = 101 },
                    SpecializationNames = new Dictionary<string, List<string>>
                    {
                        { language, new List<string> { "Cardiology", "Internal Medicine" } }
                    },
                    Phone = "1234567890",
                    Email = "john.doe@example.com",
                    ImageUrl = "doctor-image.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    MaxFollowUps = 2,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                }
            };
            
            _redisMock.Setup(c => c.GetAllDoctorsAsync()).ReturnsAsync((List<DoctorBasicDto>?)null);
            _unitOfWorkMock.Setup(u => u.DoctorRepository.GetAllForCacheAsync()).ReturnsAsync(doctorList);

            var query = new GetAdminDoctorsQuery(new DoctorFilterOptions
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Act
            var result = await _handler_GetAdminDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Items.Count, Is.EqualTo(1));
            Assert.That(result.Data.Items[0].Id, Is.EqualTo(2));
            _redisMock.Verify(c => c.SetAllDoctorsAsync(doctorList), Times.Once);
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_WhenPageNumberExceedsTotalPages()
        {
            // Arrange
            var doctorList = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 3,
                    FirstNames = new() { ["en-US"] = "James", ["ar-EG"] = "جيمس" },
                    TranslationIds = new() { ["en-US"] = 100, ["ar-EG"] = 101 }
                }
            };

            _redisMock.Setup(c => c.GetAllDoctorsAsync()).ReturnsAsync(doctorList);

            var query = new GetAdminDoctorsQuery(new DoctorFilterOptions
            {
                PageNumber = 5,
                PageSize = 1 // Max 2 pages with 2 doctors
            });

            // Act
            var result = await _handler_GetAdminDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Does.Contain("Page number"));
        }

        [Test]
        public async Task Handle_Should_MapAllTranslations_WhenCalledByAdmin()
        {
            // Arrange
            var language = "en-US";
            var doctorList = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 2,
                    UserId = "user-123",
                    FirstNames = new Dictionary<string, string> { ["en-US"] = "James", ["ar-EG"] = "جيمس" },
                    LastNames = new Dictionary<string, string> { ["en-US"] = "Doe", ["ar-EG"] = "ديو" },
                    Bio = new Dictionary<string, string> { ["en-US"] = "Experienced doctor", ["ar-EG"] = "دكتور خبير" },
                    TranslationIds = new Dictionary<string, int> { ["en-US"] = 100, ["ar-EG"] = 101 },
                    SpecializationNames = new Dictionary<string, List<string>>
                    {
                        { language, new List<string> { "Cardiology", "Internal Medicine" } }
                    },
                    Phone = "1234567890",
                    Email = "john.doe@example.com",
                    ImageUrl = "doctor-image.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    MaxFollowUps = 2,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                }
            };

            _redisMock.Setup(c => c.GetAllDoctorsAsync()).ReturnsAsync(doctorList);

            var query = new GetAdminDoctorsQuery(new DoctorFilterOptions
            {
                PageNumber = 1,
                PageSize = 10
            });

            // Act
            var result = await _handler_GetAdminDoctorsQueryHandler.Handle(query, CancellationToken.None);

            // Assert
            var dto = result.Data.Items.First();
            Assert.That(result.Succeeded, Is.True);

            var enTranslation = dto.Translations.FirstOrDefault(t => t.Language == "en-US");
            var arTranslation = dto.Translations.FirstOrDefault(t => t.Language == "ar-EG");

            Assert.That(enTranslation, Is.Not.Null);
            Assert.That(enTranslation.Id, Is.EqualTo(100));
            Assert.That(enTranslation.FirstName, Is.EqualTo("James"));

            Assert.That(arTranslation, Is.Not.Null);
            Assert.That(arTranslation.Id, Is.EqualTo(101));
            Assert.That(arTranslation.FirstName, Is.EqualTo("جيمس"));
        }


        #endregion

        #region Get Top 5
        [Test]
        public async Task Handle_ShouldReturnCachedTop5Doctors_WhenCacheExists()
        {
            // Arrange
            var cachedDoctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 1,
                    FirstNames = new() { ["en-US"] = "John" },
                    LastNames = new() { ["en-US"] = "Doe" },
                    Bio = new() { ["en-US"] = "Expert in heart surgery" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Cardiology" } },
                    Email = "john@example.com",
                    Phone = "123",
                    ImageUrl = "img1.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                },
                new DoctorBasicDto
                {
                    Id = 2,
                    FirstNames = new() { ["en-US"] = "Jane" },
                    LastNames = new() { ["en-US"] = "Smith" },
                    Bio = new() { ["en-US"] = "Skin specialist" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Dermatology" } },
                    Email = "jane@example.com",
                    Phone = "456",
                    ImageUrl = "img2.jpg",
                    InitialFee = 120,
                    FollowUpFee = 60,
                    YearsOfExperience = 8,
                    RatingPoints = 40,
                    NumberOfRatings = 8
                }
            };
            _redisMock.Setup(c => c.GetTop5DoctorsAsync()).ReturnsAsync(cachedDoctors);

            var handler = new GetTop5DoctorsHandler(_redisMock.Object, _doctorRepoMock.Object);

            // Act
            var result = await handler.Handle(new GetTop5DoctorsQuery("en"), default);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(cachedDoctors));
            _doctorRepoMock.Verify(repo => repo.GetAllForCacheAsync(), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldFilterTop5FromAllDoctorsCache_WhenTop5NotCached()
        {
            // Arrange
            var cachedDoctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 1,
                    FirstNames = new() { ["en-US"] = "John" },
                    LastNames = new() { ["en-US"] = "Doe" },
                    Bio = new() { ["en-US"] = "Expert in heart surgery" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Cardiology" } },
                    Email = "john@example.com",
                    Phone = "123",
                    ImageUrl = "img1.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                },
                new DoctorBasicDto
                {
                    Id = 2,
                    FirstNames = new() { ["en-US"] = "Jane" },
                    LastNames = new() { ["en-US"] = "Smith" },
                    Bio = new() { ["en-US"] = "Skin specialist" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Dermatology" } },
                    Email = "jane@example.com",
                    Phone = "456",
                    ImageUrl = "img2.jpg",
                    InitialFee = 120,
                    FollowUpFee = 60,
                    YearsOfExperience = 8,
                    RatingPoints = 40,
                    NumberOfRatings = 8
                }
            };
            _redisMock.Setup(c => c.GetTop5DoctorsAsync()).ReturnsAsync((List<DoctorBasicDto>?)null);
            _redisMock.Setup(c => c.GetAllDoctorsAsync()).ReturnsAsync(cachedDoctors);

            var handler = new GetTop5DoctorsHandler(_redisMock.Object, _doctorRepoMock.Object);

            // Act
            var result = await handler.Handle(new GetTop5DoctorsQuery("en-US"), default);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Count, Is.EqualTo(2)); // Because we added 2 in cache

            var first = result.Data[0];
            var second = result.Data[1];

            // Ensure sorted by rating (RatingPoints / NumberOfRatings)
            var firstAvg = first.RatingPoints / (double)first.NumberOfRatings;
            var secondAvg = second.RatingPoints / (double)second.NumberOfRatings;

            Assert.That(firstAvg, Is.GreaterThanOrEqualTo(secondAvg));

            // Verify top 5 were set to cache
            _redisMock.Verify(c => c.SetTop5DoctorsAsync(It.IsAny<List<DoctorBasicDto>>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldFetchDoctorsFromDbAndCache_WhenBothCachesMissing()
        {
            // Arrange
            var cachedDoctors = new List<DoctorBasicDto>
            {
                new DoctorBasicDto
                {
                    Id = 1,
                    FirstNames = new() { ["en-US"] = "John" },
                    LastNames = new() { ["en-US"] = "Doe" },
                    Bio = new() { ["en-US"] = "Expert in heart surgery" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Cardiology" } },
                    Email = "john@example.com",
                    Phone = "123",
                    ImageUrl = "img1.jpg",
                    InitialFee = 100,
                    FollowUpFee = 50,
                    YearsOfExperience = 10,
                    RatingPoints = 45,
                    NumberOfRatings = 10
                },
                new DoctorBasicDto
                {
                    Id = 2,
                    FirstNames = new() { ["en-US"] = "Jane" },
                    LastNames = new() { ["en-US"] = "Smith" },
                    Bio = new() { ["en-US"] = "Skin specialist" },
                    SpecializationNames = new() { ["en-US"] = new List<string> { "Dermatology" } },
                    Email = "jane@example.com",
                    Phone = "456",
                    ImageUrl = "img2.jpg",
                    InitialFee = 120,
                    FollowUpFee = 60,
                    YearsOfExperience = 8,
                    RatingPoints = 40,
                    NumberOfRatings = 8
                }
            };
            _redisMock.Setup(c => c.GetTop5DoctorsAsync()).ReturnsAsync((List<DoctorBasicDto>?)null);
            _redisMock.Setup(c => c.GetAllDoctorsAsync()).ReturnsAsync((List<DoctorBasicDto>?)null);

            _doctorRepoMock.Setup(r => r.GetAllForCacheAsync()).ReturnsAsync(cachedDoctors);

            var handler = new GetTop5DoctorsHandler(_redisMock.Object, _doctorRepoMock.Object);

            // Act
            var result = await handler.Handle(new GetTop5DoctorsQuery("en-US"), default);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Count, Is.EqualTo(2));

            // Verify DB call happened
            _doctorRepoMock.Verify(r => r.GetAllForCacheAsync(), Times.Once);

            // Verify both caching methods called
            _redisMock.Verify(c => c.SetAllDoctorsAsync(It.IsAny<List<DoctorBasicDto>>()), Times.Once);
            _redisMock.Verify(c => c.SetTop5DoctorsAsync(It.IsAny<List<DoctorBasicDto>>()), Times.Once);
        }

        #endregion
    }
}
