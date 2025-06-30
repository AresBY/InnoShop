using MediatR;

namespace InnoShop.Products.Application.Features.Products.Commands
{
    public sealed class DeleteProductCommand : IRequest<Unit>
    {
        public Guid Id { get; init; }
    }
}
