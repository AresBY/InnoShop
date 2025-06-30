using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Features.Users.Commands;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _configurationMock = new Mock<IConfiguration>();

        // Мокаем конфигурацию
        _configurationMock
            .Setup(config => config["ProductService:BaseUrl"])
            .Returns("http://localhost/");

        // Мокаем HTTP-клиент с успешным ответом
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.IsAny<HttpRequestMessage>(),
               ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        _httpClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        _handler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        var command = new UpdateUserCommand { Id = Guid.NewGuid() };
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"User with id {command.Id} not found.", ex.Message);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesUserAndCallsHttpPost()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "OldName",
            Email = "old@example.com",
            Role = UserRole.User,
            IsActive = false
        };

        var command = new UpdateUserCommand
        {
            Id = userId,
            Name = "NewName",
            Email = "new@example.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.Equal("NewName", user.Name);
        Assert.Equal("new@example.com", user.Email);
        Assert.Equal(UserRole.Admin, user.Role);
        Assert.True(user.IsActive);

        _userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _httpClientFactoryMock.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoStatusChange_DoesNotCallHttpPost()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "OldName",
            Email = "old@example.com",
            Role = UserRole.User,
            IsActive = true
        };

        var command = new UpdateUserCommand
        {
            Id = userId,
            IsActive = true
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        _httpClientFactoryMock.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
