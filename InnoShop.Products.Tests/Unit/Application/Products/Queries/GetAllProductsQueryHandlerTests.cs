using AutoMapper;
using FluentAssertions;
using InnoShop.Products.Application.Dtos;
using InnoShop.Products.Application.Features.Products.Queries;
using InnoShop.Products.Application.Interfaces.Repositories;
using Moq;

namespace InnoShop.Products.Tests.Application.Features.Products.Queries
{
    public class GetAllProductsQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllProductsQueryHandler _handler;

        public GetAllProductsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllProductsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedProductDtoList()
        {
            // Arrange
            var products = new List<Domain.Entities.Product>
            {
                new Domain.Entities.Product { Id = System.Guid.NewGuid(), Name = "Product 1" },
                new Domain.Entities.Product { Id = System.Guid.NewGuid(), Name = "Product 2" }
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = products[0].Id, Name = "Product 1" },
                new ProductDto { Id = products[1].Id, Name = "Product 2" }
            };

            _repositoryMock.Setup(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            _mapperMock.Setup(m => m.Map<List<ProductDto>>(products))
                .Returns(productDtos);

            var query = new GetAllProductsQuery(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(productDtos);
            _repositoryMock.Verify(r => r.GetAllAsync(false, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<List<ProductDto>>(products), Times.Once);
        }
    }
}
