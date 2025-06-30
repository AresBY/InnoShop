using InnoShop.Users.Application.DTOs.Auth;
using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; set; } = null!;
    }
}
