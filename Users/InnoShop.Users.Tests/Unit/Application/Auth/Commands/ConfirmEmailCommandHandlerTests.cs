using FluentAssertions;
using InnoShop.Users.Application.Features.Auth.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Commands
{
    public class ConfirmEmailCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ConfirmEmailCommandHandler _handler;

        public ConfirmEmailCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new ConfirmEmailCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldConfirmEmail_WhenTokenIsValid()
        {
            // Arrange
            var email = "test@example.com";
            var token = "valid-token";

            var user = new User
            {
                Email = email,
                EmailConfirmationToken = token,
                EmailConfirmationTokenExpiryTime = DateTime.UtcNow.AddMinutes(10),
                IsEmailConfirmed = false
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new ConfirmEmailCommand
            {
                Email = email,
                Token = token
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            user.IsEmailConfirmed.Should().BeTrue();
            user.EmailConfirmationToken.Should().BeNull();
            user.EmailConfirmationTokenExpiryTime.Should().BeNull();

            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null, "valid-token", "Invalid or expired confirmation token")]               // User is null
        [InlineData("test@example.com", null, "Invalid or expired confirmation token")]           // Token mismatch
        [InlineData("test@example.com", "expired-token", "Invalid or expired confirmation token")]// Expired token
        public async Task Handle_ShouldThrowValidationException_WhenTokenIsInvalidOrExpired(string email, string token, string expectedMessage)
        {
            // Arrange
            User? user = null;
            if (email != null)
            {
                user = new User
                {
                    Email = email,
                    EmailConfirmationToken = token == "expired-token" ? "expired-token" : "valid-token",
                    EmailConfirmationTokenExpiryTime = token == "expired-token" ? DateTime.UtcNow.AddMinutes(-10) : DateTime.UtcNow.AddMinutes(10),
                    IsEmailConfirmed = false
                };
            }

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var command = new ConfirmEmailCommand
            {
                Email = email ?? "nonexistent@example.com",
                Token = token ?? "some-token"
            };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage(expectedMessage);
        }
    }
}
