using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class ForgotPasswordCommand : IRequest<Unit>
    {
        public string Email { get; set; } = null!;
    }
}
