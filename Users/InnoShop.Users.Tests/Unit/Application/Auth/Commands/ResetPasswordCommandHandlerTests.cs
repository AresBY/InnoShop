using FluentAssertions;
using InnoShop.Users.Application.Features.Auth.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Commands
{
    public class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly ResetPasswordCommandHandler _handler;

        public ResetPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _handler = new ResetPasswordCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenUserNotFound()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Email = "test@example.com",
                Token = "valid_token",
                NewPassword = "new_password"
            };
            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("Invalid or expired token");
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenTokenDoesNotMatch()
        {
            // Arrange
            var user = new User
            {
                ResetToken = "different_token",
                ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };
            var command = new ResetPasswordCommand
            {
                Email = "test@example.com",
                Token = "valid_token",
                NewPassword = "new_password"
            };
            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("Invalid or expired token");
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenTokenExpired()
        {
            // Arrange
            var user = new User
            {
                ResetToken = "valid_token",
                ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1)
            };
            var command = new ResetPasswordCommand
            {
                Email = "test@example.com",
                Token = "valid_token",
                NewPassword = "new_password"
            };
            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("Invalid or expired token");
        }

        [Fact]
        public async Task Handle_ShouldUpdatePasswordAndClearResetToken_WhenValidRequest()
        {
            // Arrange
            var user = new User
            {
                ResetToken = "valid_token",
                ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };
            var command = new ResetPasswordCommand
            {
                Email = "test@example.com",
                Token = "valid_token",
                NewPassword = "new_password"
            };
            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(x => x.HashPassword(user, command.NewPassword))
                .Returns("hashed_password");

            _userRepositoryMock
                .Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            user.PasswordHash.Should().Be("hashed_password");
            user.ResetToken.Should().BeNull();
            user.ResetTokenExpiryTime.Should().BeNull();
            _userRepositoryMock.Verify();
        }
    }
}
