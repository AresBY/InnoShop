using FluentAssertions;
using InnoShop.Products.Application.Features.Products.Commands;
using InnoShop.Products.Application.Interfaces.Repositories;
using Moq;

namespace InnoShop.Products.Tests.Application.Features.Products.Commands
{
    public class UpdateProductsVisibilityCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly UpdateProductsVisibilityCommandHandler _handler;

        public UpdateProductsVisibilityCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _handler = new UpdateProductsVisibilityCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateProductsVisibilityAndReturnUnit()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var isVisible = true;
            var command = new UpdateProductsVisibilityCommand(userId, isVisible);

            _repositoryMock
                .Setup(r => r.UpdateProductsVisibility(userId, isVisible, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.UpdateProductsVisibility(userId, isVisible, It.IsAny<CancellationToken>()), Times.Once);
            result.Should().Be(MediatR.Unit.Value);
        }
    }
}
