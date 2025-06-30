using InnoShop.Users.Application.DTOs.User;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Queries
{
    public sealed class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }
}
