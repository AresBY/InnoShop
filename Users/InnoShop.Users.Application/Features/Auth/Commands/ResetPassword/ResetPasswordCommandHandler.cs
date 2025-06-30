using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || user.ResetToken != request.Token || user.ResetTokenExpiryTime < DateTime.UtcNow)
                throw new ValidationException("Invalid or expired token");

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiryTime = null;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Unit.Value;
        }

        Task IRequestHandler<ResetPasswordCommand>.Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }

}
