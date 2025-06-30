using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AccountConfirmationSettings _confirmationSettings;
        private readonly IMediator _mediator;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IOptions<AccountConfirmationSettings> confirmationSettings,
            IMediator mediator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _confirmationSettings = confirmationSettings.Value;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
                throw new ValidationException("A user with this email already exists.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmationToken = Guid.NewGuid().ToString(),
                EmailConfirmationTokenExpiryTime = DateTime.UtcNow.AddMinutes(_confirmationSettings.TokenLifetimeMinutes)
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddAsync(user, cancellationToken);

            await _mediator.Send(new SendEmailConfirmationCommand { Email = user.Email }, cancellationToken);

            return user.Id;
        }
    }
}
