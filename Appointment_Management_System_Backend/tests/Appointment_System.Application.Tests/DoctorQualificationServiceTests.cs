using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.Services.Implementaions;
using Appointment_System.Domain.Entities;
using System.Numerics;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;

namespace Appointment_System.Application.Tests
{

    [TestFixture]
    public class DoctorQualificationServiceTests
    {
        private Mock<IDoctorQualificationRepository> _mockRepo;
        private Mock<IDoctorRepository> _mockDoctorRepo;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private DoctorQualificationService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IDoctorQualificationRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Set up the UnitOfWork to return the mocked repositories
            _mockUnitOfWork.Setup(u => u.QualificationRepository).Returns(_mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.Doctors).Returns(_mockDoctorRepo.Object);

            _service = new DoctorQualificationService(_mockUnitOfWork.Object);
        }



        // GetByDoctorId Tests
        [Test]
        public void GetByDoctorIdAsync_WithNullId_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.GetByDoctorIdAsync(null));
        }

        [Test]
        public async Task GetByDoctorIdAsync_ReturnsList()
        {
            var doctorId = "doc123";
            _mockRepo.Setup(r => r.GetByDoctorIdAsync(doctorId)).ReturnsAsync(new List<DoctorQualification>
            {
                new DoctorQualification { Id = 1, QualificationName = "MBBS", DoctorId = doctorId }
            });

            var result = await _service.GetByDoctorIdAsync(doctorId);
            Assert.That(result.Count, Is.EqualTo(1));
        }
        //***********************************************************************

        // GetById Tests
        [Test]
        public void GetByIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(0));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DoctorQualification)null);
            var result = await _service.GetByIdAsync(1);
            Assert.That(result, Is.Null);
        }
        //************************************************************************

        // Add Tests
        [Test]
        public async Task AddAsync_WithValidInput_ReturnsDto()
        {
            var dto = new CreateDoctorQualificationDto
            {
                DoctorId = "doc1",
                QualificationName = "MBBS",
                IssuingInstitution = "Uni",
                YearEarned = 2020
            };

            var entity = dto.ToEntity();

            _mockDoctorRepo.Setup(d => d.GetDoctorByIdAsync(dto.DoctorId)).ReturnsAsync(new User());
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DoctorQualification>())).ReturnsAsync(1);

            var result = await _service.AddAsync(dto);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.QualificationName, Is.EqualTo(dto.QualificationName));
        }

        [Test]
        public void AddAsync_WithNullDto_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddAsync(null));
        }

        [Test]
        public void AddAsync_WithMissingDoctor_ThrowsKeyNotFoundException()
        {
            var dto = new CreateDoctorQualificationDto { DoctorId = "x" };
            _mockDoctorRepo.Setup(d => d.GetDoctorByIdAsync(dto.DoctorId)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddAsync(dto));
        }

        [Test]
        public void AddAsync_ShouldThrow_WhenYearEarnedIsInvalid()
        {
            var dto = new CreateDoctorQualificationDto
            {
                DoctorId = "doctor123",
                QualificationName = "Test",
                YearEarned = 1800 // Invalid year
            };

            // Mock the doctor repository to return true (simulate doctor exists)
            _mockDoctorRepo.Setup(d => d.GetDoctorByIdAsync(dto.DoctorId)).ReturnsAsync(new User());

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.AddAsync(dto));
        }

        //************************************************************************

        // Update Tests
        [Test]
        public void UpdateAsync_WithInvalidId_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(0, new UpdateDoctorQualificationDto()));
        }

        [Test]
        public void UpdateAsync_WithNullDto_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(1, null));
        }

        [Test]
        public void UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DoctorQualification)null);
            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, new UpdateDoctorQualificationDto()));
        }

        [Test]
        public async Task UpdateAsync_Valid()
        {
            var existing = new DoctorQualification { Id = 1, QualificationName = "Old" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<DoctorQualification>())).Returns(Task.CompletedTask);

            var dto = new UpdateDoctorQualificationDto
            {
                QualificationName = "New",
                IssuingInstitution = "Updated Uni",
                YearEarned = 2022
            };

            await _service.UpdateAsync(1, dto);
            Assert.That(existing.QualificationName, Is.EqualTo("New"));
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenYearEarnedIsInFuture()
        {
            var dto = new UpdateDoctorQualificationDto
            {
                QualificationName = "Updated Name",
                YearEarned = DateTime.UtcNow.Year + 5
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DoctorQualification
            {
                Id = 1,
                DoctorId = "doctor123",
                YearEarned = 2000,
                QualificationName = "Old"
            });

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.UpdateAsync(1, dto));
        }

        //************************************************************************

        // Delete Tests
        [Test]
        public void DeleteAsync_WithInvalidId_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(0));
        }

        [Test]
        public void DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((DoctorQualification)null);
            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(1));
        }

        [Test]
        public async Task DeleteAsync_Valid()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new DoctorQualification { Id = 1 });
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);
            Assert.Pass();
        }
    }
}
