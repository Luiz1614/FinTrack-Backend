using AutoMapper;
using Fintrack.Contracts.DTOs.Category;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Repositories.Interfaces;

namespace FinTrack.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> AddCategoryAsync(CategoryCreateDto categoryCreateDto)
    {
        var entity = _mapper.Map<Category>(categoryCreateDto);

        var savedEntity = await _categoryRepository.AddCategoryAsync(entity);

        return _mapper.Map<CategoryDto>(savedEntity);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        await _categoryRepository.DeleteCategoryAsync(id);
        return true;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var entities = await _categoryRepository.GetAllCategoriesAsync();

        return _mapper.Map<IEnumerable<CategoryDto>>(entities);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var entity = await _categoryRepository.GetCategoryByIdAsync(id);

        return _mapper.Map<CategoryDto>(entity);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
    {
        var entity = _mapper.Map<Category>(categoryUpdateDto);

        var updatedEntity = await _categoryRepository.UpdateCategoryAsync(entity);

        return _mapper.Map<CategoryDto>(updatedEntity);
    }
}
