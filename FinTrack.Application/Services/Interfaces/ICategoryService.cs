using Fintrack.Contracts.DTOs.Category;

namespace FinTrack.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddCategoryAsync(CategoryCreateDto categoryCreateDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
    }
}