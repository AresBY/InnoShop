using AutoMapper;
using InnoShop.Users.Application.DTOs.User;
using InnoShop.Users.Application.Features.Users.Commands;
using InnoShop.Users.Domain.Entities;

namespace InnoShop.Users.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserCommand, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // хешируем отдельно
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        }
    }
}
