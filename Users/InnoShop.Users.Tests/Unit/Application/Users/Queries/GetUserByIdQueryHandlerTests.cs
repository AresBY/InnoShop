using AutoMapper;
using FluentAssertions;
using InnoShop.Users.Application.DTOs.User;
using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Features.Users.Queries;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using Moq;

namespace InnoShop.Users.Tests.Unit.Application.Users.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                   .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedUserDto_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = "John Doe",
                Email = "john@example.com",
                Role = UserRole.Admin,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(user);

            var handler = new GetUserByIdQueryHandler(mockRepo.Object, _mapper);
            var query = new GetUserByIdQuery { Id = userId };

            var expected = _mapper.Map<UserDto>(user);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((User?)null);

            var handler = new GetUserByIdQueryHandler(mockRepo.Object, _mapper);
            var query = new GetUserByIdQuery { Id = userId };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                     .WithMessage($"User with id {userId} not found.");
        }
    }
}
