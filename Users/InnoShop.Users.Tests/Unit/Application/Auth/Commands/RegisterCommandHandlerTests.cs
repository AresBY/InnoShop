using FluentAssertions;
using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Features.Auth.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Commands
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Mock<IOptions<AccountConfirmationSettings>> _confirmationSettingsMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RegisterCommandHandler _handler;

        public RegisterCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _confirmationSettingsMock = new Mock<IOptions<AccountConfirmationSettings>>();
            _mediatorMock = new Mock<IMediator>();

            _confirmationSettingsMock.Setup(x => x.Value)
                .Returns(new AccountConfirmationSettings { TokenLifetimeMinutes = 30 });

            _handler = new RegisterCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _confirmationSettingsMock.Object,
                _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenUserWithEmailAlreadyExists()
        {
            // Arrange
            var existingEmail = "test@example.com";

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(existingEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Email = existingEmail });

            var command = new RegisterCommand
            {
                Email = existingEmail,
                Name = "Test User",
                Password = "Password123"
            };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("A user with this email already exists.");
        }
    }
}
