using AutoMapper;
using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // When creating from DTO, don't overwrite identity/ownership fields
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore());

            // On update, ignore id/user and don't map nulls so partial updates are safe
            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryPagingDto>();
        }
    }
}
