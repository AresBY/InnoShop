using AutoMapper;
using InnoShop.Products.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InnoShop.Products.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(
            IProductRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (existingProduct == null)
            {
                throw new Exception($"Product with id {request.Id} not found.");
            }

            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userIdGuid))
            {
                throw new UnauthorizedAccessException("Invalid user id format.");
            }

            if (existingProduct.CreatedByUserId != userIdGuid)
            {
                throw new UnauthorizedAccessException("You can only update your own products.");
            }

            _mapper.Map(request, existingProduct);

            await _repository.UpdateAsync(existingProduct, cancellationToken);

            return Unit.Value;
        }
    }
}
