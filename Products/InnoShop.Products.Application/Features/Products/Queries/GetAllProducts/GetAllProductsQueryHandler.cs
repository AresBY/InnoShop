using AutoMapper;
using InnoShop.Products.Application.Dtos;
using InnoShop.Products.Application.Interfaces.Repositories;
using MediatR;

namespace InnoShop.Products.Application.Features.Products.Queries
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllAsync(request.IncludeInvisible, cancellationToken);
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
