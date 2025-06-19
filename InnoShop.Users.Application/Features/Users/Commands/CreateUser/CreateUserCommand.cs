using InnoShop.Users.Domain.Entities;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = "User";
        public string PasswordHash { get; set; } = null!;
    }
}
