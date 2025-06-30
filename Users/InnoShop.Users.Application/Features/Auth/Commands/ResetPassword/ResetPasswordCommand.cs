using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class ResetPasswordCommand : IRequest
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
