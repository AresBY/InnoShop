using InnoShop.Users.Domain.Enums;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Commands
{
    public sealed class UpdateUserCommand : IRequest<Unit>
    {
        public required Guid Id { get; set; }
        public string? Name { get; set; } = null;
        public string? Email { get; set; } = null;
        public UserRole? Role { get; set; } = null;
        public bool? IsActive { get; set; } = null;
    }
}
