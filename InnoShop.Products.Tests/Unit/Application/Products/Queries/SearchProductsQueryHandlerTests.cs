using AutoMapper;
using FluentAssertions;
using InnoShop.Products.Application.Dtos;
using InnoShop.Products.Application.Features.Products.Queries;
using InnoShop.Products.Application.Interfaces.Repositories;
using Moq;

namespace InnoShop.Products.Tests.Application.Features.Products.Queries
{
    public class SearchProductsQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SearchProductsQueryHandler _handler;

        public SearchProductsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new SearchProductsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedProductDtoList()
        {
            // Arrange
            var products = new List<Domain.Entities.Product>
            {
                new Domain.Entities.Product { Id = System.Guid.NewGuid(), Name = "Product A", Price = 10 },
                new Domain.Entities.Product { Id = System.Guid.NewGuid(), Name = "Product B", Price = 20 }
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = products[0].Id, Name = "Product A", Price = 10 },
                new ProductDto { Id = products[1].Id, Name = "Product B", Price = 20 }
            };

            var query = new SearchProductsQuery
            {
                Name = "Product",
                MinPrice = 5,
                MaxPrice = 30,
                IsAvailable = true
            };

            _repositoryMock.Setup(r => r.SearchAsync(
                query.Name,
                query.MinPrice,
                query.MaxPrice,
                query.IsAvailable,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            _mapperMock.Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(productDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(productDtos);
            _repositoryMock.Verify(r => r.SearchAsync(
                query.Name,
                query.MinPrice,
                query.MaxPrice,
                query.IsAvailable,
                It.IsAny<CancellationToken>()), Times.Once);

            _mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }
    }
}
