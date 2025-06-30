using AutoMapper;
using FluentAssertions;
using InnoShop.Products.Application.Dtos;
using InnoShop.Products.Application.Features.Products.Queries;
using InnoShop.Products.Application.Interfaces.Repositories;
using Moq;

namespace InnoShop.Products.Tests.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetProductByIdQueryHandler _handler;

        public GetProductByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetProductByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ProductExists_ShouldReturnMappedProductDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Domain.Entities.Product { Id = productId, Name = "Test Product" };
            var productDto = new ProductDto { Id = productId, Name = "Test Product" };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(productDto);

            var query = new GetProductByIdQuery { Id = productId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(productDto);
            _repositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Product?)null);

            var query = new GetProductByIdQuery { Id = productId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _repositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Domain.Entities.Product>()), Times.Never);
        }
    }
}
