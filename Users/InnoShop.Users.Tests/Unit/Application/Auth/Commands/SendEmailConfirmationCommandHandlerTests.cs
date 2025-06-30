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
    public class SendEmailConfirmationCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IOptions<AccountConfirmationSettings>> _settingsMock;
        private readonly SendEmailConfirmationCommandHandler _handler;

        public SendEmailConfirmationCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _settingsMock = new Mock<IOptions<AccountConfirmationSettings>>();

            _settingsMock.SetupGet(s => s.Value).Returns(new AccountConfirmationSettings
            {
                TokenLifetimeMinutes = 60,
                ConfirmationBaseUrl = "https://example.com/confirm"
            });

            _handler = new SendEmailConfirmationCommandHandler(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _settingsMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var command = new SendEmailConfirmationCommand { Email = "notfound@example.com" };
            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        }

        [Fact]
        public async Task Handle_ShouldUpdateUserAndSendEmail_WhenUserFound()
        {
            // Arrange
            var user = new User
            {
                Email = "user@example.com"
            };
            var command = new SendEmailConfirmationCommand { Email = user.Email };

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _emailServiceMock
                .Setup(x => x.SendAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            user.EmailConfirmationToken.Should().NotBeNullOrEmpty();
            user.EmailConfirmationTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);

            _userRepositoryMock.Verify();
            _emailServiceMock.Verify();
        }
    }
}
