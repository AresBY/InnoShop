using AutoMapper;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InnoShop.Users.Application.Features.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
                throw new ValidationException("A user with this email already exists.");

            var user = _mapper.Map<User>(request);

            user.Id = Guid.NewGuid();
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user, cancellationToken);
            return user.Id;
        }
    }
}
