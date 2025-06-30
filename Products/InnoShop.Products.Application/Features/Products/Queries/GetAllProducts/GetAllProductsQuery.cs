using InnoShop.Products.Application.Dtos;
using MediatR;

namespace InnoShop.Products.Application.Features.Products.Queries
{
    public sealed class GetAllProductsQuery : IRequest<List<ProductDto>>
    {
        public bool IncludeInvisible { get; }

        public GetAllProductsQuery(bool includeInvisible = false)
        {
            IncludeInvisible = includeInvisible;
        }
    }
}
