using MediatR;

namespace InnoShop.Users.Application.Features.Auth.Commands
{
    public sealed class RegisterCommand : IRequest<Guid>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
