using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product is not null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Product>> GetAllAsync(bool includeInvisible, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking();

            if (!includeInvisible)
            {
                query = query.Where(p => p.IsVisible);
            }

            return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }


        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, bool? isAvailable, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (isAvailable.HasValue)
                query = query.Where(p => p.IsAvailable == isAvailable.Value);

            return await query
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateProductsVisibility(Guid createdByUserId, bool isVisible, CancellationToken cancellationToken)
        {
            var products = await _context.Products
                .Where(p => p.CreatedByUserId == createdByUserId)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                product.IsVisible = isVisible;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
