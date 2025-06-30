using FluentAssertions;
using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Features.Auth.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly IOptions<JwtSettings> _jwtSettingsOptions;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();

            _jwtSettingsOptions = Options.Create(new JwtSettings
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SecretKey = "SomeSecretKey",
                RefreshTokenLifetimeDays = 7
            });

            _handler = new RefreshTokenCommandHandler(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _jwtSettingsOptions);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            // Arrange
            var command = new RefreshTokenCommand { RefreshToken = "invalid_token" };
            _userRepositoryMock
                .Setup(r => r.GetByRefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid or expired refresh token");
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenRefreshTokenExpired()
        {
            // Arrange
            var expiredUser = new User
            {
                RefreshToken = "expired_token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1)
            };

            var command = new RefreshTokenCommand { RefreshToken = expiredUser.RefreshToken };

            _userRepositoryMock
                .Setup(r => r.GetByRefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredUser);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid or expired refresh token");
        }

        [Fact]
        public async Task Handle_ShouldReturnNewTokens_WhenRefreshTokenValid()
        {
            // Arrange
            var user = new User
            {
                RefreshToken = "valid_token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };
            var command = new RefreshTokenCommand { RefreshToken = user.RefreshToken };

            var newAccessToken = "new_access_token";
            var newRefreshToken = "new_refresh_token";

            _userRepositoryMock
                .Setup(r => r.GetByRefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(t => t.GenerateAccessToken(user))
                .Returns(newAccessToken);

            _tokenServiceMock
                .Setup(t => t.GenerateRefreshToken())
                .Returns(newRefreshToken);

            _userRepositoryMock
                .Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.AccessToken.Should().Be(newAccessToken);
            result.RefreshToken.Should().Be(newRefreshToken);

            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.RefreshToken == newRefreshToken &&
                u.RefreshTokenExpiryTime > DateTime.UtcNow), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
