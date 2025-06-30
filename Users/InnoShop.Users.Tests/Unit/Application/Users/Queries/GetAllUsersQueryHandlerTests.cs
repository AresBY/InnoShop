using AutoMapper;
using FluentAssertions;
using InnoShop.Users.Application.DTOs.User;
using InnoShop.Users.Application.Features.Users.Queries;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using Moq;

namespace InnoShop.Users.Tests.Unit.Application.Users.Queries
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                   .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnAllUsersMappedToDto()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "User1",
                    Email = "user1@example.com",
                    Role = UserRole.User,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "User2",
                    Email = "user2@example.com",
                    Role = UserRole.Admin,
                    IsActive = false,
                    IsEmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(users);

            var handler = new GetAllUsersQueryHandler(mockRepo.Object, _mapper);
            var query = new GetAllUsersQuery();

            var expectedDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedDtos);
        }
    }
}

