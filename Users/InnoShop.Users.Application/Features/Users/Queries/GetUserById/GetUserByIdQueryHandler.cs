using AutoMapper;
using InnoShop.Users.Application.DTOs.User;
using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Interfaces.Repositories;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
                throw new NotFoundException($"User with id {request.Id} not found.");

            return _mapper.Map<UserDto>(user);
        }
    }
}
