using InnoShop.Products.Application.Dtos;
using MediatR;

namespace InnoShop.Products.Application.Features.Products.Queries
{
    public sealed class SearchProductsQuery : IRequest<List<ProductDto>>
    {
        public string? Name { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public bool? IsAvailable { get; init; }
    }
}
