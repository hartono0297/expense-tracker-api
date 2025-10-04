using AutoMapper;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Common.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // When creating from DTO, don't overwrite identity/ownership fields
            CreateMap<RegisterRequestDto, User>()
                .ForMember(d => d.Id, opt => opt.Ignore());           

            // On update, ignore id/user and don't map nulls so partial updates are safe
            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.Id, opt => opt.Ignore())                
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserResponseDto>();
            CreateMap<User, UserUpdateDto>();
            CreateMap<User, RegisterResponseDto>();
        }
    }
}
