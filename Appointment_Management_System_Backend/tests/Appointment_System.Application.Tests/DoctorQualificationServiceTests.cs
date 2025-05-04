using NUnit.Framework;
using Moq;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Domain.Entities;
using System.Numerics;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Features.DoctorQualifications.Queries;
using Appointment_System.Application.Features.DoctorQualifications.Commands;

namespace Appointment_System.Application.Tests
{

    [TestFixture]
    public class DoctorQualificationServiceTests
    {
        private Mock<IDoctorQualificationRepository> _mockRepo;
        private Mock<IDoctorRepository> _mockDoctorRepo;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IDoctorQualificationRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Set up the UnitOfWork to return the mocked repositories
            _mockUnitOfWork.Setup(u => u.QualificationRepository).Returns(_mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.Doctors).Returns(_mockDoctorRepo.Object);
        }



        // GetByDoctorId Tests
        [Test]
        public void GetByDoctorIdAsync_WithNullId_ThrowsArgumentException()
        {
            var handler = new GetDoctorQualificationsByDoctorIdHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentException>(() => 
                handler.Handle(new GetDoctorQualificationsByDoctorIdQuery(0), CancellationToken.None));
        }

        [Test]
        public async Task GetByDoctorIdAsync_ReturnsList()
        {
            var doctorId = 1;
            _mockRepo.Setup(r => r.GetByDoctorIdAsync(doctorId)).ReturnsAsync(new List<Qualification>
            {
                new Qualification { Id = 1, QualificationName = "MBBS", DoctorId = doctorId }
            });

            var handler = new GetDoctorQualificationsByDoctorIdHandler(_mockUnitOfWork.Object);
            var result = await handler.Handle(new GetDoctorQualificationsByDoctorIdQuery(doctorId), CancellationToken.None);
            Assert.That(result.Count, Is.EqualTo(1));
        }
        //***********************************************************************

        // GetById Tests
        [Test]
        public void GetByIdAsync_WithInvalidId_ThrowsArgumentException()
        {
            var handler = new GetDoctorQualificationByIdHandler(_mockUnitOfWork.Object);                                                                
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(new GetDoctorQualificationByIdQuery(0), CancellationToken.None));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Qualification)null);

            var handler = new GetDoctorQualificationByIdHandler(_mockUnitOfWork.Object);
            var result = await handler.Handle(new GetDoctorQualificationByIdQuery(1), CancellationToken.None);
            Assert.That(result, Is.Null);
        }
        //************************************************************************

        // Add Tests
        [Test]
        public async Task AddAsync_WithValidInput_ReturnsDto()
        {
            var dto = new CreateDoctorQualificationDto
            {
                DoctorId = 1,
                QualificationName = "MBBS",
                IssuingInstitution = "Uni",
                YearEarned = 2020
            };

            var entity = dto.ToEntity();

            _mockDoctorRepo.Setup(d => d.GetDoctorByIdAsync(dto.DoctorId)).ReturnsAsync(new Doctor());
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Qualification>())).ReturnsAsync(1);

            var handler = new CreateDoctorQualificationHandler(_mockUnitOfWork.Object);
            var result = await handler.Handle(new CreateDoctorQualificationCommand(dto), CancellationToken.None);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.QualificationName, Is.EqualTo(dto.QualificationName));
        }

        [Test]
        public void AddAsync_WithNullDto_ThrowsArgumentNullException()
        {
            var handler = new CreateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => 
                handler.Handle(new CreateDoctorQualificationCommand(null), CancellationToken.None));
        }

        [Test]
        public void AddAsync_WithMissingDoctor_ThrowsKeyNotFoundException()
        {
            var dto = new CreateDoctorQualificationDto { DoctorId = 1 };
            _mockDoctorRepo.Setup(d => d.GetDoctorByIdAsync(dto.DoctorId)).ReturnsAsync((Doctor)null);

            var handler = new CreateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() => 
                handler.Handle(new CreateDoctorQualificationCommand(dto), CancellationToken.None));
        }

        [Test]
        public void AddAsync_ShouldThrow_WhenYearEarnedIsInvalid()
        {
            var dto = new CreateDoctorQualificationDto
            {
                DoctorId = 1,
                QualificationName = "Test",
                YearEarned = 1800 // Invalid year
            };

            // Mock the doctor repository to return true (simulate doctor exists)
            _mockDoctorRepo.Setup(d => d.GetDoctorByIdAsync(dto.DoctorId)).ReturnsAsync(new Doctor());

            var handler = new CreateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                handler.Handle(new CreateDoctorQualificationCommand(dto), CancellationToken.None));
        }

        //************************************************************************

        // Update Tests
        [Test]
        public void UpdateAsync_WithInvalidId_ThrowsArgumentException()
        {
            var handler = new UpdateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentException>(() => 
                handler.Handle(new UpdateDoctorQualificationCommand(0, new UpdateDoctorQualificationDto()), 
                    CancellationToken.None));
        }

        [Test]
        public void UpdateAsync_WithNullDto_ThrowsArgumentNullException()
        {
            var handler = new UpdateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => 
                handler.Handle(new UpdateDoctorQualificationCommand(1, null),
                    CancellationToken.None));
        }

        [Test]
        public void UpdateAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Qualification)null);

            var handler = new UpdateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new UpdateDoctorQualificationCommand(1, new UpdateDoctorQualificationDto()),
                    CancellationToken.None));
        }

        [Test]
        public async Task UpdateAsync_Valid()
        {
            var existing = new Qualification { Id = 1, QualificationName = "Old" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Qualification>())).Returns(Task.CompletedTask);

            var dto = new UpdateDoctorQualificationDto
            {
                QualificationName = "New",
                IssuingInstitution = "Updated Uni",
                YearEarned = 2022
            };

            var handler = new UpdateDoctorQualificationHandler(_mockUnitOfWork.Object);
            await handler.Handle(new UpdateDoctorQualificationCommand(1, dto),
                    CancellationToken.None);
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

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Qualification
            {
                Id = 1,
                DoctorId = 1,
                YearEarned = 2000,
                QualificationName = "Old"
            });

            var handler = new UpdateDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                handler.Handle(new UpdateDoctorQualificationCommand(1, dto),
                    CancellationToken.None));
        }

        //************************************************************************

        // Delete Tests
        [Test]
        public void DeleteAsync_WithInvalidId_ThrowsArgumentException()
        {
            var handler = new DeleteDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<ArgumentException>(() => 
                handler.Handle(new DeleteDoctorQualificationCommand(0), CancellationToken.None));
        }

        [Test]
        public void DeleteAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Qualification)null);

            var handler = new DeleteDoctorQualificationHandler(_mockUnitOfWork.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new DeleteDoctorQualificationCommand(1), CancellationToken.None));
        }

        [Test]
        public async Task DeleteAsync_Valid()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Qualification { Id = 1 });
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            var handler = new DeleteDoctorQualificationHandler(_mockUnitOfWork.Object);
            await handler.Handle(new DeleteDoctorQualificationCommand(1), CancellationToken.None);
            Assert.Pass();
        }
    }
}
