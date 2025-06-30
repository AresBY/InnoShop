using FluentAssertions;
using InnoShop.Products.Application.Features.Products.Commands;
using InnoShop.Products.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;


namespace InnoShop.Products.Tests.Unit.Application.Products.Commands
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _handler = new DeleteProductCommandHandler(
                _repositoryMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Handle_ProductExistsAndUserIsOwner_ShouldDeleteProduct()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Domain.Entities.Product
            {
                Id = productId,
                CreatedByUserId = userId
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _repositoryMock.Setup(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new DeleteProductCommand { Id = productId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            _repositoryMock.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Product)null);

            var command = new DeleteProductCommand { Id = productId };

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
            var product = new Domain.Entities.Product
            {
                Id = productId,
                CreatedByUserId = Guid.NewGuid()
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var command = new DeleteProductCommand { Id = productId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid user id.");
        }

        [Fact]
        public async Task Handle_UserIsNotOwner_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var product = new Domain.Entities.Product
            {
                Id = productId,
                CreatedByUserId = ownerId
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, otherUserId.ToString())
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new DeleteProductCommand { Id = productId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("You can only delete your own products.");
        }
    }

}
