using FluentAssertions;
using InnoShop.Products.Application.Features.Products.Commands;
using InnoShop.Products.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace InnoShop.Products.Tests.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<AutoMapper.IMapper> _mapperMock;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mapperMock = new Mock<AutoMapper.IMapper>();

            _handler = new UpdateProductCommandHandler(
                _repositoryMock.Object,
                _httpContextAccessorMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ProductExistsAndUserIsOwner_ShouldUpdateProduct()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var existingProduct = new Domain.Entities.Product
            {
                Id = productId,
                CreatedByUserId = userId
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 99.99m
            };

            _mapperMock.Setup(m => m.Map(command, existingProduct));

            _repositoryMock.Setup(r => r.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            _mapperMock.Verify(m => m.Map(command, existingProduct), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Product)null);

            var command = new UpdateProductCommand { Id = productId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage($"Product with id {productId} not found.");
        }

        [Fact]
        public async Task Handle_NoUserIdInContext_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Domain.Entities.Product
            {
                Id = productId,
                CreatedByUserId = Guid.NewGuid()
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var command = new UpdateProductCommand { Id = productId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid user id format.");
        }

        [Fact]
        public async Task Handle_UserIsNotOwner_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var existingProduct = new Domain.Entities.Product
            {
                Id = productId,
                CreatedByUserId = ownerId
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, otherUserId.ToString())
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new UpdateProductCommand { Id = productId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("You can only update your own products.");
        }
    }
}
