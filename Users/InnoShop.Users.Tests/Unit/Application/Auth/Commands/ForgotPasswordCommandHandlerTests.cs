using FluentAssertions;
using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Features.Auth.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Commands
{
    public class ForgotPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly IOptions<PasswordResetSettings> _options;
        private readonly ForgotPasswordCommandHandler _handler;

        public ForgotPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _options = Options.Create(new PasswordResetSettings { TokenExpiryMinutes = 30 });

            _handler = new ForgotPasswordCommandHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _options);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var command = new ForgotPasswordCommand { Email = "notfound@example.com" };
            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task Handle_ShouldUpdateUserAndSendEmail_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Email = "user@example.com"
            };
            var command = new ForgotPasswordCommand { Email = user.Email };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _emailServiceMock
                .Setup(svc => svc.SendAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<User>(u =>
                u.ResetToken != null &&
                u.ResetTokenExpiryTime > DateTime.UtcNow), It.IsAny<CancellationToken>()), Times.Once);

            _emailServiceMock.Verify(svc => svc.SendAsync(
                user.Email,
                "Password Reset",
                It.Is<string>(msg => msg.Contains(user.ResetToken!))), Times.Once);
        }
    }
}
