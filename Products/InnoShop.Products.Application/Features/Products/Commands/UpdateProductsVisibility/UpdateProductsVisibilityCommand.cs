using MediatR;

namespace InnoShop.Products.Application.Features.Products.Commands
{
    public sealed class UpdateProductsVisibilityCommand : IRequest<Unit>
    {
        public Guid UserId { get; }
        public bool IsVisible { get; }

        public UpdateProductsVisibilityCommand(Guid userId, bool isVisible)
        {
            UserId = userId;
            IsVisible = isVisible;
        }
    }
}
