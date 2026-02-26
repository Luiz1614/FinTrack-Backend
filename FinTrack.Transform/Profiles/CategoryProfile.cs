using AutoMapper;
using Fintrack.Contracts.DTOs.Category;
using FinTrack.Domain.Entities;

namespace FinTrack.Transform.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        // Domain -> DTO
        CreateMap<Category, CategoryDto>();
        // DTO -> Domain
        CreateMap<CategoryCreateDto, Category>();

        CreateMap<CategoryUpdateDto, Category>();
    }
}