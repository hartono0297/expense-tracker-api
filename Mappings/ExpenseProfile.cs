using AutoMapper;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.Models;

namespace ExpenseTracker.Mappings
{
    public class ExpenseProfile : Profile
    {
        public ExpenseProfile()
        {
            CreateMap<ExpenseCreateDto, Expense>();
            CreateMap<ExpenseUpdateDto, Expense>();
            CreateMap<Expense, ExpenseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty));
        }
    }
}
