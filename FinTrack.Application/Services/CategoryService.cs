using AutoMapper;
using Fintrack.Contracts.DTOs.Category;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FinTrack.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CategoryService> _logger;

    private const string ALL_CATEGORIES_CACHE_KEY = "all_categories";
    private const string CATEGORY_CACHE_KEY_PREFIX = "category_";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);
    private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(30);

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IMemoryCache memoryCache, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<CategoryDto> AddCategoryAsync(CategoryCreateDto categoryCreateDto)
    {
        var entity = _mapper.Map<Category>(categoryCreateDto);
        var savedEntity = await _categoryRepository.AddCategoryAsync(entity);

        _memoryCache.Remove(ALL_CATEGORIES_CACHE_KEY);
        _logger.LogInformation("Cache invalidado: {CacheKey} após adicionar categoria ID {CategoryId}", ALL_CATEGORIES_CACHE_KEY, savedEntity.Id);

        return _mapper.Map<CategoryDto>(savedEntity);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        await _categoryRepository.DeleteCategoryAsync(id);

        _memoryCache.Remove(ALL_CATEGORIES_CACHE_KEY);
        _memoryCache.Remove($"{CATEGORY_CACHE_KEY_PREFIX}{id}");

        _logger.LogInformation("Cache Invalidado após deletar categoria ID {CategoryId}", id);

        return true;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        if(_memoryCache.TryGetValue(ALL_CATEGORIES_CACHE_KEY, out IEnumerable<CategoryDto>? cachedCategories))
        {
            _logger.LogInformation("Categorias recuperadas do cache");
            return cachedCategories!;
        }

        _logger.LogInformation("Cache miss - buscando categorias no banco de dados");

        var entities = await _categoryRepository.GetAllCategoriesAsync();
        var dtos = _mapper.Map<IEnumerable<CategoryDto>>(entities);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheExpiration,
            SlidingExpiration = SlidingExpiration
        };

        _memoryCache.Set(ALL_CATEGORIES_CACHE_KEY, dtos, cacheOptions);
        _logger.LogInformation("Categorias armazenadas em cache por {ExpirationMinutes} minutos", CacheExpiration.TotalMinutes);

        return dtos;
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var cacheKey = $"{CATEGORY_CACHE_KEY_PREFIX}{id}";

        if(_memoryCache.TryGetValue(cacheKey, out CategoryDto? cachedCategory))
        {
            _logger.LogInformation("Categoria ID {CategoryId} recuperada do cache", id);
            return cachedCategory!;
        }

        _logger.LogInformation("Cache miss - buscando categoria Id {CategoriaId} do banco de dados", id);

        var entity = await _categoryRepository.GetCategoryByIdAsync(id);
        var dto = _mapper.Map<CategoryDto>(entity);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheExpiration,
            SlidingExpiration = SlidingExpiration
        };

        _memoryCache.Set(cacheKey, dto, cacheOptions);
        _logger.LogInformation("Categoria ID {CategoryId} armazenada no cache", id);

        return dto;
    }

    public async Task<CategoryDto> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
    {
        var entity = _mapper.Map<Category>(categoryUpdateDto);
        var updatedEntity = await _categoryRepository.UpdateCategoryAsync(entity);

        _memoryCache.Remove(ALL_CATEGORIES_CACHE_KEY);
        _memoryCache.Remove($"{CATEGORY_CACHE_KEY_PREFIX}{categoryUpdateDto.Id}");

        _logger.LogInformation("Cache invalidado após atualizar categoria ID {CategoryId}", categoryUpdateDto.Id);

        return _mapper.Map<CategoryDto>(updatedEntity);
    }
}
