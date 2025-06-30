using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly PasswordResetSettings _settings;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService,
            IOptions<PasswordResetSettings> options)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _settings = options.Value;
        }

        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpiryTime = DateTime.UtcNow.AddMinutes(_settings.TokenExpiryMinutes);

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _emailService.SendAsync(user.Email, "Password Reset", $"Use this token: {token}");

            return Unit.Value;
        }
    }
}
