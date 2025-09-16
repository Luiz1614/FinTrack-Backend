using AutoMapper;
using Fintrack.Contracts.DTOs.Category;
using FinTrack.Domain.Entities;

namespace FinTrack.Transform.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        // Domain -> DTO
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.TransactionsCount, opt => opt.MapFrom(s => s.Transactions != null ? s.Transactions.Count : 0));

        // DTO -> Domain
        CreateMap<CategoryCreateDto, Category>();

        CreateMap<CategoryUpdateDto, Category>();
    }
}