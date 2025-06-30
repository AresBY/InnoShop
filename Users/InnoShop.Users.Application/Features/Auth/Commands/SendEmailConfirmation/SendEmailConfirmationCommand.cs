using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class SendEmailConfirmationCommand : IRequest<Unit>
    {
        public string Email { get; set; } = null!;
    }
}
