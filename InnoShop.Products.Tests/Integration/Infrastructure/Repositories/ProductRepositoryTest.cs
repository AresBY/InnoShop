using FluentAssertions;
using InnoShop.Products.Domain.Entities;
using InnoShop.Products.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Tests.Integration.Infrastructure.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ProductRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Description = "Test Description",
                Price = 10,
                IsVisible = true,
                IsAvailable = true,
                CreatedByUserId = Guid.NewGuid()
            };

            await _repository.AddAsync(product, default);

            var saved = await _context.Products.FindAsync(product.Id);
            saved.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ById_ShouldRemoveProduct()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "ToDelete",
                Description = "Test Description"
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(product.Id, default);

            var result = await _context.Products.FindAsync(product.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ByEntity_ShouldRemoveProduct()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "ToDelete",
                Description = "Test Description"
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(product, default);

            var result = await _context.Products.FindAsync(product.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByVisibility()
        {
            await _context.Products.AddRangeAsync(
                new Product { Id = Guid.NewGuid(), Name = "Visible", Description = "Desc", IsVisible = true },
                new Product { Id = Guid.NewGuid(), Name = "Hidden", Description = "Desc", IsVisible = false });
            await _context.SaveChangesAsync();

            var visibleOnly = await _repository.GetAllAsync(false, default);
            visibleOnly.Should().ContainSingle(p => p.Name == "Visible");

            var all = await _repository.GetAllAsync(true, default);
            all.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProduct()
        {
            var id = Guid.NewGuid();
            await _context.Products.AddAsync(new Product { Id = id, Name = "ById", Description = "Desc" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(id, default);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task SearchAsync_ShouldApplyFilters()
        {
            await _context.Products.AddRangeAsync(
                new Product { Name = "Apple", Description = "Desc", Price = 10, IsAvailable = true },
                new Product { Name = "Banana", Description = "Desc", Price = 20, IsAvailable = false });
            await _context.SaveChangesAsync();

            var result = await _repository.SearchAsync("App", null, null, true, default);
            result.Should().ContainSingle(p => p.Name == "Apple");

            result = await _repository.SearchAsync(null, 15, 25, null, default);
            result.Should().ContainSingle(p => p.Name == "Banana");
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistChanges()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "OldName", Description = "Desc" };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            product.Name = "NewName";
            await _repository.UpdateAsync(product, default);

            var updated = await _context.Products.FindAsync(product.Id);
            updated?.Name.Should().Be("NewName");
        }

        [Fact]
        public async Task UpdateProductsVisibility_ShouldUpdateVisibility()
        {
            var userId = Guid.NewGuid();
            await _context.Products.AddRangeAsync(
                new Product { Name = "A", Description = "Desc", IsVisible = true, CreatedByUserId = userId },
                new Product { Name = "B", Description = "Desc", IsVisible = true, CreatedByUserId = userId });
            await _context.SaveChangesAsync();

            await _repository.UpdateProductsVisibility(userId, false, default);

            var products = await _context.Products.Where(p => p.CreatedByUserId == userId).ToListAsync();
            products.All(p => p.IsVisible == false).Should().BeTrue();
        }
    }
}
