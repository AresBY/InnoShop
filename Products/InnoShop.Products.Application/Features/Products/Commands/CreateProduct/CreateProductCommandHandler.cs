using AutoMapper;
using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InnoShop.Products.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(
            IProductRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid or missing user id.");
            }

            var product = _mapper.Map<Product>(request);
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.CreatedByUserId = userId;
            product.IsVisible = true;

            await _repository.AddAsync(product, cancellationToken);

            return product.Id;
        }
    }
}
