using AutoMapper;
using InnoShop.Products.Application.Dtos;
using InnoShop.Products.Application.Interfaces.Repositories;
using MediatR;

namespace InnoShop.Products.Application.Features.Products.Queries
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public SearchProductsQueryHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repository.SearchAsync(
                request.Name,
                request.MinPrice,
                request.MaxPrice,
                request.IsAvailable,
                cancellationToken);

            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
