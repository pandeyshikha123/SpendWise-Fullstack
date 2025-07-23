using AutoMapper;
using SpendWiseAPI.Models;
using SpendWiseAPI.DTOs;

namespace SpendWiseAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserRegisterDTO, User>();

            CreateMap<SignUpDto, User>();
            CreateMap<User, UserResponseDTO>();

            // Expense mappings
            CreateMap<Expense, ExpenseResponseDTO>();
            CreateMap<ExpenseCreateDTO, Expense>();

            // Category mappings
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<CategoryDTO, Category>();
        }
    }
}
