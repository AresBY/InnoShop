using InnoShop.Users.Application.Interfaces.Repositories;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public ConfirmEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null ||
                user.EmailConfirmationToken != request.Token ||
                user.EmailConfirmationTokenExpiryTime == null ||
                user.EmailConfirmationTokenExpiryTime < DateTime.UtcNow)
            {
                throw new ValidationException("Invalid or expired confirmation token");
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return Unit.Value;
        }
    }
}
