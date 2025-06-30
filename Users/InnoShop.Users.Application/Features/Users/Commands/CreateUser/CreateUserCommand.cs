using InnoShop.Users.Domain.Enums;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Commands
{
    public sealed class CreateUserCommand : IRequest<Guid>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.User;
        public string Password { get; set; } = null!;
    }
}
