using AutoMapper;
using FluentAssertions;
using InnoShop.Users.Application.Features.Users.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Tests.Unit.Application.Users.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IMapper _mapper;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserCommand, User>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            });

            _mapper = configuration.CreateMapper();

            _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenUserAlreadyExists()
        {
            // Arrange
            var existingUser = new User { Email = "existing@example.com" };
            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(existingUser.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var command = new CreateUserCommand
            {
                Name = "John",
                Email = existingUser.Email,
                Password = "Secret123",
                Role = UserRole.Admin
            };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("A user with this email already exists.");
        }

        [Fact]
        public async Task Handle_ShouldCreateUserAndReturnId_WhenEmailIsUnique()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            _userRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var command = new CreateUserCommand
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "StrongPass123",
                Role = UserRole.User
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBe(Guid.Empty);
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
