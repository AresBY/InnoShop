using InnoShop.Products.Application.Interfaces.Repositories;
using MediatR;

namespace InnoShop.Products.Application.Features.Products.Commands
{
    public class UpdateProductsVisibilityCommandHandler : IRequestHandler<UpdateProductsVisibilityCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductsVisibilityCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(UpdateProductsVisibilityCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.UpdateProductsVisibility(request.UserId, request.IsVisible, cancellationToken);
            return Unit.Value;
        }
    }
}
