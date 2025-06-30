using AutoMapper;
using FluentAssertions;
using InnoShop.Products.Application.Features.Products.Commands;
using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace InnoShop.Products.Tests.Unit.Application.Products.Commands
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateProductCommandHandler(
                _repositoryMock.Object,
                _httpContextAccessorMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldAddProductAndReturnId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));
            httpContext.User = claimsPrincipal;

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var command = new CreateProductCommand
            {
                // Здесь можно заполнить необходимые свойства команды
            };

            var mappedProduct = new Product();
            _mapperMock.Setup(m => m.Map<Product>(command)).Returns(mappedProduct);

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p =>
                p == mappedProduct &&
                p.CreatedByUserId == userId &&
                p.IsVisible == true &&
                p.CreatedAt <= DateTime.UtcNow
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoUserIdInContext_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()); // User пустой

            var command = new CreateProductCommand();

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid or missing user id.");
        }
    }
}
