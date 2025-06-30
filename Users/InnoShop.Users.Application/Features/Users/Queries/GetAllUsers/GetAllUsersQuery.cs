using InnoShop.Users.Application.DTOs.User;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Queries
{
    public sealed class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }
}
