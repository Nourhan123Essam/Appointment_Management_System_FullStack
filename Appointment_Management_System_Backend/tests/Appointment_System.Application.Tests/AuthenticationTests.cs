using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Localization;
using Appointment_System.Application.Tests.Helpers;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Appointment_System.Application.Features.Authentication.Commands;
using Appointment_System.Application.DTOs.Authentication;
using Microsoft.EntityFrameworkCore;


namespace Appointment_System.Application.Tests
{
    [TestFixture]
    public class AuthRepositoryTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private Mock<IRedisService> _redisServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ISessionService> _sessionServiceMock;
        private Mock<ILocalizationService> _localizerMock;
        private Mock<IConfiguration> _configurationMock;

        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<IAuthenticationRepository> _authenticationRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        private AuthenticationRepository _authRepo;

        private ApplicationDbContext _dbContext;
        [TearDown]
        public void TearDown()
        {
            _dbContext?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = MockHelpers.MockUserManager<IdentityUser>();
            _signInManagerMock = MockHelpers.MockSignInManager<IdentityUser>();
            _configurationMock = new Mock<IConfiguration>();
            _redisServiceMock = new Mock<IRedisService>();
            _sessionServiceMock = new Mock<ISessionService>();
            _localizerMock = new Mock<ILocalizationService>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _authenticationRepositoryMock = new Mock<IAuthenticationRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();

            // Create real DbContext with InMemory
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique DB per test
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _authRepo = new AuthenticationRepository(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _configurationMock.Object,
                _redisServiceMock.Object,
                _sessionServiceMock.Object,
                _localizerMock.Object,
                _patientRepositoryMock.Object,
                _dbContext // This is your real in-memory ApplicationDbContext
            );

            // Replace the mocked version with the real instance:
            _authenticationRepositoryMock = new Mock<IAuthenticationRepository>();
            _authenticationRepositoryMock.Setup(r => r.GetUserIdByEmailAsync(It.IsAny<string>()))
                                         .Returns<string>(email => _authRepo.GetUserIdByEmailAsync(email));


            // Then:
            _unitOfWorkMock.Setup(u => u.Authentication).Returns(_authenticationRepositoryMock.Object);

        }

        #region Login
        [Test]
        public async Task Login_ShouldFail_WhenEmailIsInvalid()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync("fake@example.com"))
                            .ReturnsAsync((IdentityUser)null);
            _localizerMock.Setup(l => l["InvalidEmail"]).Returns("البريد الإلكتروني غير صالح");

            // Act
            var result = await _authRepo.Login("fake@example.com", "password");

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("البريد الإلكتروني غير صالح"));
        }


        [Test]
        public async Task Login_ShouldFail_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", Email = "user@example.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email))
                            .ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "wrongpassword", false))
                              .ReturnsAsync(SignInResult.Failed);
            _localizerMock.Setup(l => l["InvalidPassword"]).Returns("كلمة المرور غير صحيحة");

            // Act
            var result = await _authRepo.Login(user.Email, "wrongpassword");

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("كلمة المرور غير صحيحة"));
        }

        [Test]
        public async Task Login_ShouldSucceed_WhenCredentialsAreCorrect()
        {
            // Arrange
            var user = new IdentityUser
            {
                Id = "1",
                Email = "user@example.com",
                UserName = "user@example.com"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "Password123@", false))
                              .ReturnsAsync(SignInResult.Success);

            _redisServiceMock.Setup(x => x.SetRefreshTokenAsync(user.Id, It.IsAny<string>(), It.IsAny<TimeSpan>()))
                             .Returns(Task.CompletedTask);

            _sessionServiceMock.Setup(x => x.CreateSessionAsync(user.Id))
                               .ReturnsAsync("session123");

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey1234567890!@#$%^&*()"); // 32+ characters
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // Act
            var result = await _authRepo.Login(user.Email, "Password123@");

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.AccessToken, Is.Not.Null);
            Assert.That(result.Data.SessionId, Is.EqualTo("session123"));
        }

        #endregion


        #region Register
        [Test]
        public async Task Register_ShouldSucceed_WhenEmailIsNewAndUserCreatedSuccessfully()
        {
            var patient = GetTestPatient();
            var password = "Test@123";

            _userManagerMock.Setup(x => x.FindByEmailAsync(patient.Email))
                .ReturnsAsync((IdentityUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"))
                .ReturnsAsync(IdentityResult.Success);
            _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>()))
                      .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                           .ReturnsAsync(1);


            var result = await _authRepo.Register(patient, password);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo(_localizerMock.Object["RegisterSuccess"]));
        }

        [Test]
        public async Task Register_ShouldFail_WhenEmailAlreadyExists()
        {
            var patient = GetTestPatient();

            _userManagerMock.Setup(x => x.FindByEmailAsync(patient.Email))
                .ReturnsAsync(new IdentityUser());

            _localizerMock.Setup(l => l["EmailAlreadyExists"]).Returns("Email is already registered");

            var result = await _authRepo.Register(patient, "Test@123");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email is already registered"));
        }

        [Test]
        public async Task Register_ShouldFail_WhenCreateAsyncFails()
        {
            var patient = GetTestPatient();

            _userManagerMock.Setup(x => x.FindByEmailAsync(patient.Email))
                .ReturnsAsync((IdentityUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid password" }));
            _localizerMock.Setup(l => l["IdentityCreationFailed"]).Returns("Failed to create user account");

            var result = await _authRepo.Register(patient, "badpass");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to create user account"));
        }

        [Test]
        public async Task Register_AddToRoleFails_ReturnsRoleAssignmentFailed()
        {
            // Arrange
            var patient = new Patient { Email = "test@example.com", Phone = "12345" };
            var password = "Password123!";

            _userManagerMock.Setup(u => u.FindByEmailAsync(patient.Email))
                .ReturnsAsync((IdentityUser)null);

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"))
                .ReturnsAsync(IdentityResult.Failed());

            _localizerMock.Setup(l => l["RoleAssignmentFailed"])
                .Returns(new LocalizedString("RoleAssignmentFailed", "Role assignment failed."));

            // Act
            var result = await _authRepo.Register(patient, password);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Role assignment failed."));
        }

        [Test]
        public async Task Register_AddPatientThrows_ReturnsDatabaseSaveFailed()
        {
            // Arrange
            var patient = new Patient { Email = "test@example.com", Phone = "12345" };
            var password = "Password123!";

            _userManagerMock.Setup(u => u.FindByEmailAsync(patient.Email))
                .ReturnsAsync((IdentityUser)null);

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"))
                .ReturnsAsync(IdentityResult.Success);

            _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>()))
                .ThrowsAsync(new Exception("DB failed"));

            _localizerMock.Setup(l => l["DatabaseSaveFailed"])
                .Returns(new LocalizedString("DatabaseSaveFailed", "Failed to save to database."));

            // Act
            var result = await _authRepo.Register(patient, password);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to save to database."));
        }



        private Patient GetTestPatient() => new()
        {
            Email = "test@example.com",
            Phone = "01012345678",
            FirstName = "Hanen",
            LastName = "Yasser",
            Gender = "Female",
            DateOfBirth = new DateTime(1998, 5, 1),
            Address = "Cairo"
        };
        #endregion


        #region Logout Tests

        [Test]
        public async Task Logout_InvalidRefreshToken_ReturnsFail()
        {
            var command = new LogoutCommand("invalid_token");
            _redisServiceMock.Setup(r => r.GetUserIdByRefreshTokenAsync("invalid_token"))
                .ReturnsAsync(string.Empty);

            _localizerMock.Setup(l => l["InvalidOrExpiredRefreshToken"])
                .Returns(new LocalizedString("InvalidOrExpiredRefreshToken", "Invalid or expired refresh token."));

            var handler = new LogoutCommandHandler(_redisServiceMock.Object, _sessionServiceMock.Object, _localizerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid or expired refresh token."));
        }

        [Test]
        public async Task Logout_ValidRefreshToken_ReturnsSuccess()
        {
            var command = new LogoutCommand("valid_token");
            var userId = "user123";

            _redisServiceMock.Setup(r => r.GetUserIdByRefreshTokenAsync("valid_token"))
                .ReturnsAsync(userId);
            _redisServiceMock.Setup(r => r.DeleteRefreshTokenAsync(userId))
                .Returns(Task.CompletedTask);
            _sessionServiceMock.Setup(s => s.RemoveSessionAsync(userId))
                .Returns(Task.CompletedTask);

            _localizerMock.Setup(l => l["LogoutSuccessful"])
                .Returns(new LocalizedString("LogoutSuccessful", "Logout successful."));

            var handler = new LogoutCommandHandler(_redisServiceMock.Object, _sessionServiceMock.Object, _localizerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Logout successful."));
        }

        #endregion


        #region RefreshToken Tests
        [Test]
        public async Task RefreshToken_InvalidRefreshToken_ReturnsFail()
        {
            // Arrange
            var command = new RefreshTokenCommand("invalid_token", "session123");
            _redisServiceMock.Setup(r => r.GetUserIdByRefreshTokenAsync("invalid_token"))
                .ReturnsAsync(string.Empty);

            _localizerMock.Setup(l => l["InvalidOrExpiredRefreshToken"])
                .Returns(new LocalizedString("InvalidOrExpiredRefreshToken", "Invalid or expired refresh token."));

            var handler = new RefreshTokenCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _sessionServiceMock.Object, _localizerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid or expired refresh token."));
        }

        [Test]
        public async Task RefreshToken_UserNotFound_ReturnsFail()
        {
            // Arrange
            var command = new RefreshTokenCommand("valid_token", "session123");
            var userId = "user123";

            _redisServiceMock.Setup(r => r.GetUserIdByRefreshTokenAsync("valid_token"))
                .ReturnsAsync(userId);

            _unitOfWorkMock.Setup(u => u.Authentication.GenerateTokenAsync(userId))
                .ReturnsAsync((string?)null);

            _localizerMock.Setup(l => l["UserNotFound"])
                .Returns(new LocalizedString("UserNotFound", "User not found."));

            var handler = new RefreshTokenCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _sessionServiceMock.Object, _localizerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not found."));
        }

        [Test]
        public async Task RefreshToken_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var refreshToken = "valid_token";
            var userId = "user123";
            var sessionId = "session123";
            var newAccessToken = "new_access_token";
            var newRefreshToken = "new_refresh_token";

            var command = new RefreshTokenCommand(refreshToken, sessionId);

            _redisServiceMock.Setup(r => r.GetUserIdByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(userId);

            _unitOfWorkMock.Setup(u => u.Authentication.GenerateTokenAsync(userId))
                .ReturnsAsync(newAccessToken);

            _unitOfWorkMock.Setup(u => u.Authentication.GenerateRefreshToken())
                .Returns(newRefreshToken);

            _redisServiceMock.Setup(r => r.DeleteRefreshTokenAsync(refreshToken))
                .Returns(Task.CompletedTask);

            _redisServiceMock.Setup(r => r.SetRefreshTokenAsync(newRefreshToken, userId, It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _sessionServiceMock.Setup(s => s.ValidateSessionAsync(userId, sessionId))
                .ReturnsAsync(true);

            _sessionServiceMock.Setup(s => s.ValidateAndExtendSessionAsync(userId, sessionId))
                .ReturnsAsync(true);

            var handler = new RefreshTokenCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _sessionServiceMock.Object, _localizerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.AccessToken, Is.EqualTo(newAccessToken));
            Assert.That(result.Data.RefreshToken, Is.EqualTo(newRefreshToken));
            Assert.That(result.Data.SessionId, Is.EqualTo(sessionId));
            Assert.That(result.Data.ExpiresIn, Is.EqualTo(900)); // 15 minutes
        }


        #endregion


        #region RequestPasswordReset Tests
        [Test]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var email = "notfound@example.com";
            var command = new RequestPasswordResetCommand(email);

            _unitOfWorkMock.Setup(u => u.Authentication.GetUserIdByEmailAsync(email))
                .ReturnsAsync((string?)null);

            _localizerMock.Setup(l => l["UserNotFound"])
                .Returns(new LocalizedString("UserNotFound", "User not found."));

            var handler = new RequestPasswordResetHandler(
                _unitOfWorkMock.Object,
                _redisServiceMock.Object,
                _emailServiceMock.Object,
                _localizerMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not found."));
        }


        //[Test]
        //public async Task Handle_ValidEmail_StoresTokenAndSendsEmail_ReturnsSuccess()
        //{
        //    // Arrange
        //    var email = "test@example.com";
        //    var userId = "user123";
        //    var command = new RequestPasswordResetCommand(email);
        //    var generatedToken = Guid.NewGuid().ToString();

        //    _unitOfWorkMock.Setup(u => u.Authentication.GetUserIdByEmailAsync(email))
        //        .ReturnsAsync(userId);

        //    _redisServiceMock.Setup(r => r.SetResetPasswordTokenAsync(It.IsAny<string>(), userId, It.IsAny<TimeSpan>()))
        //        .Returns(Task.CompletedTask);

        //    _emailServiceMock.Setup(e => e.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>()))
        //        .Returns(Task.CompletedTask);

        //    _localizerMock.Setup(l => l["ResetPasswordSubject"])
        //        .Returns(new LocalizedString("ResetPasswordSubject", "Reset your password"));

        //    Environment.SetEnvironmentVariable("CLIENT_URL", "https://client.com");

        //    var handler = new RequestPasswordResetHandler(
        //        _unitOfWorkMock.Object,
        //        _redisServiceMock.Object,
        //        _emailServiceMock.Object,
        //        _localizerMock.Object
        //    );

        //    // Act
        //    var result = await handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.That(result.Succeeded, Is.True);
        //    Assert.That(result.Data, Is.EqualTo(""));

        //    _redisServiceMock.Verify(r => r.SetResetPasswordTokenAsync(It.IsAny<string>(), userId, TimeSpan.FromMinutes(10)), Times.Once);

        //    _emailServiceMock.Verify(e =>
        //        e.SendEmailAsync(email, "Reset your password", It.Is<string>(s => s.Contains("reset-password"))), Times.Once);
        //}

        [Test]
        public async Task Handle_ValidEmail_StoresTokenAndSendsEmail_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var userId = "user123";
            var command = new RequestPasswordResetCommand(email);
            var generatedToken = Guid.NewGuid().ToString();

            _unitOfWorkMock.Setup(u => u.Authentication.GetUserIdByEmailAsync(email))
                .ReturnsAsync(userId);

            _redisServiceMock.Setup(r => r.SetResetPasswordTokenAsync(It.IsAny<string>(), userId, It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _emailServiceMock.Setup(e => e.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _localizerMock.Setup(l => l["ResetPasswordSubject"])
                .Returns(new LocalizedString("ResetPasswordSubject", "Reset your password"));

            _localizerMock.Setup(l => l["ResetPasswordHeader"])
                .Returns(new LocalizedString("ResetPasswordHeader", "Reset Your Password"));

            _localizerMock.Setup(l => l["ResetPasswordInstruction"])
                .Returns(new LocalizedString("ResetPasswordInstruction", "Click the button below to reset your password."));

            _localizerMock.Setup(l => l["ResetPasswordButton"])
                .Returns(new LocalizedString("ResetPasswordButton", "Reset Password"));

            _localizerMock.Setup(l => l["ResetPasswordIgnoreNote"])
                .Returns(new LocalizedString("ResetPasswordIgnoreNote", "If you didn't request this, you can ignore this email."));

            Environment.SetEnvironmentVariable("CLIENT_URL", "https://client.com");

            var handler = new RequestPasswordResetHandler(
                _unitOfWorkMock.Object,
                _redisServiceMock.Object,
                _emailServiceMock.Object,
                _localizerMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.EqualTo(""));

            _redisServiceMock.Verify(r => r.SetResetPasswordTokenAsync(It.IsAny<string>(), userId, TimeSpan.FromMinutes(10)), Times.Once);

            _emailServiceMock.Verify(e =>
                e.SendEmailAsync(email, "Reset your password", It.Is<string>(s => s.Contains("reset-password"))), Times.Once);
        }


        #endregion


        #region ResetPassword Tests

        [Test]
        public async Task ResetPassword_PasswordsDoNotMatch_ReturnsFail()
        {
            var dto = new ResetPasswordDto
            {
                NewPassword = "123456",
                ConfirmPassword = "654321",
                Token = "reset-token"
            };

            var command = new ResetPasswordCommand(dto);

            _localizerMock.Setup(l => l["PasswordsDoNotMatch"])
                .Returns(new LocalizedString("PasswordsDoNotMatch", "Passwords do not match."));

            var handler = new ResetPasswordCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _localizerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Passwords do not match."));
        }

        [Test]
        public async Task ResetPassword_InvalidToken_ReturnsFail()
        {
            var dto = new ResetPasswordDto
            {
                NewPassword = "password123",
                ConfirmPassword = "password123",
                Token = "invalid-token"
            };

            var command = new ResetPasswordCommand(dto);

            _redisServiceMock.Setup(r => r.GetUserIdByResetPasswordTokenAsync("invalid-token"))
                .ReturnsAsync((string?)null);

            _localizerMock.Setup(l => l["InvalidOrExpiredToken"])
                .Returns(new LocalizedString("InvalidOrExpiredToken", "Invalid or expired token."));

            var handler = new ResetPasswordCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _localizerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid or expired token."));
        }

        [Test]
        public async Task ResetPassword_UserNotFound_ReturnsFail()
        {
            var dto = new ResetPasswordDto
            {
                NewPassword = "password123",
                ConfirmPassword = "password123",
                Token = "valid-token"
            };

            var command = new ResetPasswordCommand(dto);

            _redisServiceMock.Setup(r => r.GetUserIdByResetPasswordTokenAsync("valid-token"))
                .ReturnsAsync("user123");

            _authenticationRepositoryMock.Setup(a => a.UpdatePasswordAsync("user123", "password123"))
                .ReturnsAsync(false);

            _localizerMock.Setup(l => l["UserNotFound"])
                .Returns(new LocalizedString("UserNotFound", "User not found."));

            var handler = new ResetPasswordCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _localizerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not found."));
        }

        [Test]
        public async Task ResetPassword_Success_ReturnsSuccess()
        {
            var dto = new ResetPasswordDto
            {
                NewPassword = "password123",
                ConfirmPassword = "password123",
                Token = "valid-token"
            };

            var command = new ResetPasswordCommand(dto);

            _redisServiceMock.Setup(r => r.GetUserIdByResetPasswordTokenAsync("valid-token"))
                .ReturnsAsync("user123");

            _authenticationRepositoryMock.Setup(a => a.UpdatePasswordAsync("user123", "password123"))
                .ReturnsAsync(true);

            _redisServiceMock.Setup(r => r.DeleteResetPasswordTokenAsync("valid-token"))
                .Returns(Task.CompletedTask);

            _localizerMock.Setup(l => l["PasswordResetSuccess"])
                .Returns(new LocalizedString("PasswordResetSuccess", "Password reset successfully."));

            var handler = new ResetPasswordCommandHandler(_unitOfWorkMock.Object, _redisServiceMock.Object, _localizerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Message, Is.EqualTo("Password reset successfully."));
        }

        #endregion

    }
}
