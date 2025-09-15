using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> UpdateCategoryAsync(Category category);
    }
}