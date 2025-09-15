using FinTrack.Domain.Entities;

namespace FinTrack.Infraestructure.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> AddAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<int> SaveChangesAsync();
        void Update(Category category);
    }
}