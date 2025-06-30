using FluentAssertions;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using InnoShop.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Users.Tests.Integration.Infrastructure.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new UserRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "add@example.com",
                Name = "Add User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "dummy_hash"
            };

            await _repository.AddAsync(user, CancellationToken.None);

            var savedUser = await _context.Users.FindAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser.Email.Should().Be("add@example.com");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "user1@example.com",
                Name = "User One",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "hash1"
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Email = "user2@example.com",
                Name = "User Two",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "hash2"
            };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            var users = await _repository.GetAllAsync(CancellationToken.None);

            users.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "getbyid@example.com",
                Name = "GetById User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "dummy_hash"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var foundUser = await _repository.GetByIdAsync(user.Id, CancellationToken.None);

            foundUser.Should().NotBeNull();
            foundUser.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var foundUser = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

            foundUser.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "update@example.com",
                Name = "Update User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "dummy_hash"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            user.Name = "Updated Name";
            user.IsActive = false;

            await _repository.UpdateAsync(user, CancellationToken.None);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            updatedUser?.Name.Should().Be("Updated Name");
            updatedUser?.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser_WhenExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "delete@example.com",
                Name = "Delete User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "dummy_hash"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(user.Id, CancellationToken.None);

            var deletedUser = await _context.Users.FindAsync(user.Id);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotThrow_WhenUserNotExists()
        {
            var nonExistentId = Guid.NewGuid();

            Func<Task> act = async () => await _repository.DeleteAsync(nonExistentId, CancellationToken.None);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "email@example.com",
                Name = "Email User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "dummy_hash"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var foundUser = await _repository.GetByEmailAsync(user.Email, CancellationToken.None);

            foundUser.Should().NotBeNull();
            foundUser.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
        {
            var foundUser = await _repository.GetByEmailAsync("notfound@example.com", CancellationToken.None);

            foundUser.Should().BeNull();
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "refresh@example.com",
                Name = "Refresh User",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = "dummy_hash",
                RefreshToken = "valid_refresh_token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var foundUser = await _repository.GetByRefreshTokenAsync("valid_refresh_token", CancellationToken.None);

            foundUser.Should().NotBeNull();
            foundUser.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldReturnNull_WhenNotExists()
        {
            var foundUser = await _repository.GetByRefreshTokenAsync("invalid_token", CancellationToken.None);

            foundUser.Should().BeNull();
        }
    }
}
