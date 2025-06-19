using InnoShop.Users.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace InnoShop.Users.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<IEnumerable<User>>
    {
    }
}
