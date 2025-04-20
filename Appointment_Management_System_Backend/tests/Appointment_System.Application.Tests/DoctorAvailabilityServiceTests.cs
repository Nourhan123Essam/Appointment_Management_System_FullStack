using NUnit.Framework;
using Moq;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.Services.Implementaions;
using Appointment_System.Domain.Entities;
using System.Numerics;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;

namespace Appointment_System.Application.Tests
{
    [TestFixture]
    public class DoctorAvailabilityServiceTests
    {
        private Mock<IDoctorAvailabilityRepository> _availabilityRepoMock;
        private Mock<IDoctorRepository> _doctorRepoMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private DoctorAvailabilityService _service;

        [SetUp]
        public void Setup()
        {
            _availabilityRepoMock = new Mock<IDoctorAvailabilityRepository>();
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Setup the unit of work to return the mocked repositories
            _unitOfWorkMock.Setup(u => u.AvailabilityRepository).Returns(_availabilityRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Doctors).Returns(_doctorRepoMock.Object);

            _service = new DoctorAvailabilityService(_unitOfWorkMock.Object);
        }


        // Add Tests
        #region AddAsync Tests

        [Test]
        public void AddAsync_NullDto_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddAsync(null));
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

            Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
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

            Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
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

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddAsync(dto));
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

            var result = await _service.AddAsync(dto);

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

            var result = await _service.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DoctorAvailability)null);

            var result = await _service.GetByIdAsync(1);

            Assert.That(result, Is.Null);
        }
        #endregion

        // GetByDoctorId Test
        #region GetByDoctorIdAsync
        [Test]
        public void GetByDoctorIdAsync_ShouldThrow_WhenDoctorIdIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetByDoctorIdAsync(null));
        }

        [Test]
        public void GetByDoctorIdAsync_ShouldThrow_WhenDoctorIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetByDoctorIdAsync(" "));
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

            var result = await _service.GetByDoctorIdAsync(doctorId);
            Assert.That(result.Count, Is.EqualTo(2));
        }
        #endregion

        // Delete Tests
        #region DeleteAsync
        [Test]
        public void DeleteAsync_ShouldThrow_WhenAvailabilityNotFound()
        {
            _availabilityRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((DoctorAvailability)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1));
        }

        [Test]
        public async Task DeleteAsync_ShouldDelete_WhenAvailabilityExists()
        {
            _availabilityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DoctorAvailability());

            await _service.DeleteAsync(1);

            _availabilityRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }
        #endregion

        // Update Tests
        #region UpdateAsync
        [Test]
        public void UpdateAsync_ShouldThrow_WhenDtoIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(1, null));
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenStartTimeGreaterOrEqualEndTime()
        {
            var dto = new UpdateDoctorAvailabilityDto
            {
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(9)
            };

            Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(1, dto));
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

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, dto));
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

            await _service.UpdateAsync(1, dto);

            _availabilityRepoMock.Verify(r => r.UpdateAsync(It.IsAny<DoctorAvailability>()), Times.Once);
        }
        #endregion
    }
}
