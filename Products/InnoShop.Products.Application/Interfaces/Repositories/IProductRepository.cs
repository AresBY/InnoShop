using InnoShop.Products.Domain.Entities;

namespace InnoShop.Products.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(bool includeInvisible, CancellationToken cancellationToken);

        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task AddAsync(Product product, CancellationToken cancellationToken);

        Task UpdateAsync(Product product, CancellationToken cancellationToken);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task DeleteAsync(Product product, CancellationToken cancellationToken);

        Task<List<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, bool? isAvailable, CancellationToken cancellationToken);

        Task UpdateProductsVisibility(Guid ownerId, bool isVisible, CancellationToken cancellationToken);
    }
}
