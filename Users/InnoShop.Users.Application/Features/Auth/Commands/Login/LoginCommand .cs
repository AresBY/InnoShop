using InnoShop.Users.Application.DTOs.Auth;
using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = "free2107@mail.ru";
        public string Password { get; set; } = "adminadmin";
    }
}
