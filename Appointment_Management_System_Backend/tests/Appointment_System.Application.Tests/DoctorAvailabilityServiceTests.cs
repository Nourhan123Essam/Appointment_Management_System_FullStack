using NUnit.Framework;
using Moq;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Domain.Entities;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Features.DoctorAvailabilities.Commands;
using Appointment_System.Application.Features.DoctorAvailabilities.Queries;

namespace Appointment_System.Application.Tests
{
    [TestFixture]
    public class DoctorAvailabilityServiceTests
    {
        private Mock<IDoctorAvailabilityRepository> _availabilityRepoMock;
        private Mock<IDoctorRepository> _doctorRepoMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        [SetUp]
        public void Setup()
        {
            _availabilityRepoMock = new Mock<IDoctorAvailabilityRepository>();
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Setup the unit of work to return the mocked repositories
            _unitOfWorkMock.Setup(u => u.AvailabilityRepository).Returns(_availabilityRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);
        }


        // Add Tests
        #region AddAsync Tests

        [Test]
        public void AddAsync_NullDto_ThrowsArgumentNullException()
        {
            var hadler = new CreateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => 
                hadler.Handle(new CreateDoctorAvailabilityCommand(null), CancellationToken.None));
        }

        [Test]
        public void AddAsync_EmptyDoctorId_ThrowsArgumentException()
        {
            var dto = new CreateDoctorAvailabilityDto
            {
                DoctorId = "",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17),
                DayOfWeek = DayOfWeek.Monday
            };

            var hadler = new CreateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => 
                hadler.Handle(new CreateDoctorAvailabilityCommand(dto), CancellationToken.None));
        }

        [Test]
        public void AddAsync_StartTimeAfterEndTime_ThrowsArgumentException()
        {
            var dto = new CreateDoctorAvailabilityDto
            {
                DoctorId = "doc1",
                StartTime = TimeSpan.FromHours(18),
                EndTime = TimeSpan.FromHours(9),
                DayOfWeek = DayOfWeek.Monday
            };

            _doctorRepoMock.Setup(r => r.GetDoctorByIdAsync("doc1")).ReturnsAsync(new User());

            var hadler = new CreateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => 
                hadler.Handle(new CreateDoctorAvailabilityCommand(dto), CancellationToken.None));
        }

        [Test]
        public void AddAsync_DoctorNotExists_ThrowsKeyNotFoundException()
        {
            var dto = new CreateDoctorAvailabilityDto
            {
                DoctorId = "notfound",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17),
                DayOfWeek = DayOfWeek.Monday
            };

            _doctorRepoMock.Setup(r => r.GetDoctorByIdAsync("notfound")).ReturnsAsync((User)null);

            var hadler = new CreateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() => 
                hadler.Handle(new CreateDoctorAvailabilityCommand(dto), CancellationToken.None));
        }

        [Test]
        public async Task AddAsync_ValidInput_ReturnsAvailabilityDto()
        {
            var dto = new CreateDoctorAvailabilityDto
            {
                DoctorId = "doc1",
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17),
                DayOfWeek = DayOfWeek.Monday
            };

            var availability = new DoctorAvailability
            {
                Id = 1,
                DoctorId = "doc1",
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DayOfWeek = dto.DayOfWeek
            };

            _doctorRepoMock.Setup(r => r.GetDoctorByIdAsync("doc1")).ReturnsAsync(new User());
            _availabilityRepoMock.Setup(r => r.AddAsync(It.IsAny<DoctorAvailability>())).ReturnsAsync(1);

            var hadler = new CreateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            var result = await hadler.Handle(new CreateDoctorAvailabilityCommand(dto), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.DoctorId, Is.EqualTo("doc1"));
        }

        #endregion

        // GetById Tests
        #region GetByIdAsync
        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsDto()
        {
            var availability = new DoctorAvailability
            {
                Id = 1,
                DoctorId = "doc1",
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(12),
                DayOfWeek = DayOfWeek.Tuesday
            };

            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(availability);

            var handler = new GetDoctorAvailabilityByIdHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new GetDoctorAvailabilityByIdQuery(1), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DoctorAvailability)null);

            var handler = new GetDoctorAvailabilityByIdHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new GetDoctorAvailabilityByIdQuery(1), CancellationToken.None);

            Assert.That(result, Is.Null);
        }
        #endregion

        // GetByDoctorId Test
        #region GetByDoctorIdAsync
        [Test]
        public void GetByDoctorIdAsync_ShouldThrow_WhenDoctorIdIsNull()
        {
            var handler = new GetDoctorAvailabilitiesByDoctorIdHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => 
                handler.Handle(new GetDoctorAvailabilitiesByDoctorIdQuery(null), CancellationToken.None));
        }

        [Test]
        public void GetByDoctorIdAsync_ShouldThrow_WhenDoctorIdIsEmpty()
        {
            var handler = new GetDoctorAvailabilitiesByDoctorIdHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(new GetDoctorAvailabilitiesByDoctorIdQuery(" "), CancellationToken.None));
        }
        [Test]
        public async Task GetByDoctorIdAsync_ExistingDoctorId_ReturnsListOfDtos()
        {
            var doctorId = "doc1";
            var availabilitys = new List<DoctorAvailability>() {
                new DoctorAvailability
                {
                    Id = 1,
                    DoctorId = doctorId,
                    StartTime = TimeSpan.FromHours(8),
                    EndTime = TimeSpan.FromHours(12),
                    DayOfWeek = DayOfWeek.Tuesday
                },
                new DoctorAvailability
                {
                    Id = 2,
                    DoctorId = doctorId,
                    StartTime = TimeSpan.FromHours(8),
                    EndTime = TimeSpan.FromHours(10),
                    DayOfWeek = DayOfWeek.Friday
                }
            };

            _doctorRepoMock.Setup(r => r.GetDoctorByIdAsync(doctorId)).ReturnsAsync(new User { Id = doctorId });
            _availabilityRepoMock.Setup(r => r.GetByDoctorIdAsync(doctorId)).ReturnsAsync(availabilitys);

            var handler = new GetDoctorAvailabilitiesByDoctorIdHandler(_unitOfWorkMock.Object);
            var result = await handler.Handle(new GetDoctorAvailabilitiesByDoctorIdQuery(doctorId), CancellationToken.None);
            Assert.That(result.Count, Is.EqualTo(2));
        }
        #endregion

        // Delete Tests
        #region DeleteAsync
        [Test]
        public void DeleteAsync_ShouldThrow_WhenAvailabilityNotFound()
        {
            _availabilityRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((DoctorAvailability)null);

            var handler = new DeleteDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() => 
                handler.Handle(new DeleteDoctorAvailabilityCommand(1), CancellationToken.None));
        }

        [Test]
        public async Task DeleteAsync_ShouldDelete_WhenAvailabilityExists()
        {
            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DoctorAvailability());

            var handler = new DeleteDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            await handler.Handle(new DeleteDoctorAvailabilityCommand(1), CancellationToken.None);

            _availabilityRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }
        #endregion

        // Update Tests
        #region UpdateAsync
        [Test]
        public void UpdateAsync_ShouldThrow_WhenDtoIsNull()
        {
            var handler = new UpdateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => 
                handler.Handle(new UpdateDoctorAvailabilityCommand(1, null), CancellationToken.None));
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenStartTimeGreaterOrEqualEndTime()
        {
            var dto = new UpdateDoctorAvailabilityDto
            {
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(9)
            };

            var handler = new UpdateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(new UpdateDoctorAvailabilityCommand(1, dto), CancellationToken.None));
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenAvailabilityNotFound()
        {
            var dto = new UpdateDoctorAvailabilityDto
            {
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(10)
            };

            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DoctorAvailability)null);

            var handler = new UpdateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new UpdateDoctorAvailabilityCommand(1, dto), CancellationToken.None));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdate_WhenValid()
        {
            var dto = new UpdateDoctorAvailabilityDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = TimeSpan.FromHours(8),
                EndTime = TimeSpan.FromHours(10)
            };

            var availability = new DoctorAvailability();

            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(availability);

            var handler = new UpdateDoctorAvailabilityCommandHandler(_unitOfWorkMock.Object);
            await handler.Handle(new UpdateDoctorAvailabilityCommand(1, dto), CancellationToken.None);

            _availabilityRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DoctorAvailability>()), Times.Once);
        }
        #endregion
    }
}
