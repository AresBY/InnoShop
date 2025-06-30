using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class ConfirmEmailCommand : IRequest<Unit>
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
