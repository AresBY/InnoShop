using InnoShop.Products.Application.Dtos;
using MediatR;

namespace InnoShop.Products.Application.Features.Products.Queries
{
    public sealed class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public Guid Id { get; set; }
    }
}
