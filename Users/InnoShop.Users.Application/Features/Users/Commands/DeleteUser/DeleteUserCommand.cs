using MediatR;

namespace InnoShop.Users.Application.Features.Users.Commands
{
    public sealed class DeleteUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
