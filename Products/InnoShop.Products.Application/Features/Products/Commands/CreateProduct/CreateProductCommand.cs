using MediatR;

namespace InnoShop.Products.Application.Features.Products.Commands
{
    public sealed class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public decimal Price { get; init; }
        public bool IsAvailable { get; init; }
        public Guid CreatedByUserId { get; init; }
    }
}
