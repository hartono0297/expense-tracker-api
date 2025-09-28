using AutoMapper;
using ExpenseTracker.DTOs.CategoryDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
                
        }
    }
}
