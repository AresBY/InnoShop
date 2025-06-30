using FluentAssertions;
using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Features.Auth.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly JwtSettings _jwtSettings;

        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _jwtSettings = new JwtSettings
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SecretKey = "SomeSecretKey",
                RefreshTokenLifetimeDays = 7
            };
            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(x => x.Value).Returns(_jwtSettings);

            _handler = new LoginCommandHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _jwtOptionsMock.Object,
                _passwordHasherMock.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var command = new LoginCommand { Email = "test@example.com", Password = "password" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User { Email = "test@example.com", PasswordHash = "hashedPassword" };
            var command = new LoginCommand { Email = user.Email, Password = "wrongPassword" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, command.Password))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthResponseAndUpdatesUser()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.MinValue
            };

            var command = new LoginCommand { Email = user.Email, Password = "correctPassword" };

            var expectedAccessToken = "access_token";
            var expectedRefreshToken = "refresh_token";

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock.Setup(ph => ph.VerifyHashedPassword(user, user.PasswordHash, command.Password))
                .Returns(PasswordVerificationResult.Success);

            _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(user))
                .Returns(expectedAccessToken);

            _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken())
                .Returns(expectedRefreshToken);

            _userRepositoryMock.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(expectedAccessToken);
            result.RefreshToken.Should().Be(expectedRefreshToken);
            user.RefreshToken.Should().Be(expectedRefreshToken);
            user.RefreshTokenExpiryTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays), TimeSpan.FromSeconds(5));

            _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
