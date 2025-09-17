using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Data.Context.Interfaces;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infraestructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDataContext _context;

    public CategoryRepository(IDataContext context)
    {
        _context = context;
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await GetCategoryByIdAsync(id);
        if (category == null)
        {
            return false;
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        var entity = await GetCategoryByIdAsync(category.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Category with id {category.Id} not found.");
        }

        entity.Title = category.Title;
        entity.Description = category.Description;

        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
}