using InnoShop.Shared.DTOs.Users;
using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;


namespace InnoShop.Users.Application.Features.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;


        public UpdateUserCommandHandler(IUserRepository userRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<Unit> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
            if (user == null)
                throw new NotFoundException($"User with id {command.Id} not found.");

            if (command.Name is not null)
                user.Name = command.Name;

            if (command.Email is not null)
                user.Email = command.Email;

            if (command.Role is not null)
                user.Role = (UserRole)command.Role;

            if (command.IsActive.HasValue && user.IsActive != command.IsActive.Value)
            {
                user.IsActive = command.IsActive.Value;

                var baseUrl = _configuration["ProductService:BaseUrl"];
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(baseUrl!);

                var response = await client.PostAsJsonAsync("api/product", new UserStatusChanged(user.Id, user.IsActive), cancellationToken);
                response.EnsureSuccessStatusCode();
            }

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }
    }
}
