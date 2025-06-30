using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOptions<AccountConfirmationSettings> _settings;

        public SendEmailConfirmationCommandHandler(IUserRepository userRepository, IEmailService emailService,
            IOptions<AccountConfirmationSettings> settings)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _settings = settings;
        }

        public async Task<Unit> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            user.EmailConfirmationToken = Guid.NewGuid().ToString();
            user.EmailConfirmationTokenExpiryTime = DateTime.UtcNow.AddMinutes(_settings.Value.TokenLifetimeMinutes);

            await _userRepository.UpdateAsync(user, cancellationToken);

            var confirmationLink = $"{_settings.Value.ConfirmationBaseUrl}?email={user.Email}&token={user.EmailConfirmationToken}";

            await _emailService.SendAsync(user.Email, "Confirm your email", $"Click the link to confirm your email: {confirmationLink}");

            return Unit.Value;
        }
    }
}
