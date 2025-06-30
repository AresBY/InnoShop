using InnoShop.Users.Application.Exceptions;
using InnoShop.Users.Application.Interfaces.Repositories;
using MediatR;

namespace InnoShop.Users.Application.Features.Users.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException($"User with id {request.Id} not found.");
            }

            await _userRepository.DeleteAsync(user.Id, cancellationToken);

            return Unit.Value;
        }
    }
}
