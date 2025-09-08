using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Repositories;

public class CategoryRepository : ICategoryRepositories
{
    private readonly DataContext _context;

    public CategoryRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Category> AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }
        _context.Categories.Remove(category);
        return true;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public void Update(Category category)
    {
        _context.Entry(category).State = EntityState.Modified;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}