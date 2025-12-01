using AutoMapper;
using Fintrack.Contracts.DTOs.Category;
using FinTrack.Application.Services;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Moq;

namespace FinTrack.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockMapper = new Mock<IMapper>();
        _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddCategoryAsync_ShouldReturnCategoryDto_WhenCategoryIsCreatedSuccessfully()
    {
        // Arrange
        var categoryCreateDto = new CategoryCreateDto
        {
            Title = "Food",
            Description = "Food and beverages expenses"
        };

        var categoryEntity = new Category
        {
            Id = 1,
            Title = "Food",
            Description = "Food and beverages expenses"
        };

        var categoryDto = new CategoryDto
        {
            Id = 1,
            Title = "Food",
            Description = "Food and beverages expenses",
            TransactionsCount = 0
        };

        _mockMapper.Setup(m => m.Map<Category>(categoryCreateDto)).Returns(categoryEntity);
        _mockCategoryRepository.Setup(r => r.AddCategoryAsync(categoryEntity)).ReturnsAsync(categoryEntity);
        _mockMapper.Setup(m => m.Map<CategoryDto>(categoryEntity)).Returns(categoryDto);

        // Act
        var result = await _categoryService.AddCategoryAsync(categoryCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryDto.Id, result.Id);
        Assert.Equal(categoryDto.Title, result.Title);
        Assert.Equal(categoryDto.Description, result.Description);
        Assert.Equal(categoryDto.TransactionsCount, result.TransactionsCount);

        _mockMapper.Verify(m => m.Map<Category>(categoryCreateDto), Times.Once);
        _mockCategoryRepository.Verify(r => r.AddCategoryAsync(categoryEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<CategoryDto>(categoryEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldReturnTrue_WhenCategoryIsDeletedSuccessfully()
    {
        // Arrange
        var categoryId = 1;

        _mockCategoryRepository.Setup(r => r.DeleteCategoryAsync(categoryId)).ReturnsAsync(true);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.True(result);
        _mockCategoryRepository.Verify(r => r.DeleteCategoryAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnListOfCategoryDtos_WhenCategoriesExist()
    {
        // Arrange
        var categoryEntities = new List<Category>
        {
            new Category { Id = 1, Title = "Food", Description = "Food expenses" },
            new Category { Id = 2, Title = "Transport", Description = "Transportation costs" },
            new Category { Id = 3, Title = "Entertainment", Description = "Fun activities" }
        };

        var categoryDtos = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Title = "Food", Description = "Food expenses", TransactionsCount = 5 },
            new CategoryDto { Id = 2, Title = "Transport", Description = "Transportation costs", TransactionsCount = 3 },
            new CategoryDto { Id = 3, Title = "Entertainment", Description = "Fun activities", TransactionsCount = 2 }
        };

        _mockCategoryRepository.Setup(r => r.GetAllCategoriesAsync()).ReturnsAsync(categoryEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categoryEntities)).Returns(categoryDtos);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal(categoryDtos[0].Title, result.First().Title);
        Assert.Equal(categoryDtos[2].Title, result.Last().Title);

        _mockCategoryRepository.Verify(r => r.GetAllCategoriesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<CategoryDto>>(categoryEntities), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        // Arrange
        var emptyCategoryEntities = new List<Category>();
        var emptyCategoryDtos = new List<CategoryDto>();

        _mockCategoryRepository.Setup(r => r.GetAllCategoriesAsync()).ReturnsAsync(emptyCategoryEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(emptyCategoryEntities)).Returns(emptyCategoryDtos);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockCategoryRepository.Verify(r => r.GetAllCategoriesAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<CategoryDto>>(emptyCategoryEntities), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnCategoryDto_WhenCategoryExists()
    {
        // Arrange
        var categoryId = 1;
        var categoryEntity = new Category
        {
            Id = categoryId,
            Title = "Health",
            Description = "Medical and health expenses"
        };

        var categoryDto = new CategoryDto
        {
            Id = categoryId,
            Title = "Health",
            Description = "Medical and health expenses",
            TransactionsCount = 7
        };

        _mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId)).ReturnsAsync(categoryEntity);
        _mockMapper.Setup(m => m.Map<CategoryDto>(categoryEntity)).Returns(categoryDto);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryDto.Id, result.Id);
        Assert.Equal(categoryDto.Title, result.Title);
        Assert.Equal(categoryDto.Description, result.Description);
        Assert.Equal(categoryDto.TransactionsCount, result.TransactionsCount);

        _mockCategoryRepository.Verify(r => r.GetCategoryByIdAsync(categoryId), Times.Once);
        _mockMapper.Verify(m => m.Map<CategoryDto>(categoryEntity), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = 999;
        Category? nullCategory = null;

        _mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId)).ReturnsAsync(nullCategory);
        _mockMapper.Setup(m => m.Map<CategoryDto>(nullCategory)).Returns((CategoryDto)null!);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.Null(result);

        _mockCategoryRepository.Verify(r => r.GetCategoryByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnUpdatedCategoryDto_WhenCategoryIsUpdatedSuccessfully()
    {
        // Arrange
        var categoryUpdateDto = new CategoryUpdateDto
        {
            Id = 1,
            Title = "Updated Category",
            Description = "Updated description"
        };

        var categoryEntity = new Category
        {
            Id = 1,
            Title = "Updated Category",
            Description = "Updated description"
        };

        var updatedCategoryDto = new CategoryDto
        {
            Id = 1,
            Title = "Updated Category",
            Description = "Updated description",
            TransactionsCount = 10
        };

        _mockMapper.Setup(m => m.Map<Category>(categoryUpdateDto)).Returns(categoryEntity);
        _mockCategoryRepository.Setup(r => r.UpdateCategoryAsync(categoryEntity)).ReturnsAsync(categoryEntity);
        _mockMapper.Setup(m => m.Map<CategoryDto>(categoryEntity)).Returns(updatedCategoryDto);

        // Act
        var result = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedCategoryDto.Id, result.Id);
        Assert.Equal(updatedCategoryDto.Title, result.Title);
        Assert.Equal(updatedCategoryDto.Description, result.Description);
        Assert.Equal(updatedCategoryDto.TransactionsCount, result.TransactionsCount);

        _mockMapper.Verify(m => m.Map<Category>(categoryUpdateDto), Times.Once);
        _mockCategoryRepository.Verify(r => r.UpdateCategoryAsync(categoryEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<CategoryDto>(categoryEntity), Times.Once);
    }

    [Fact]
    public async Task AddCategoryAsync_ShouldHandleNullDescription()
    {
        // Arrange
        var categoryCreateDto = new CategoryCreateDto
        {
            Title = "Shopping",
            Description = null
        };

        var categoryEntity = new Category
        {
            Id = 1,
            Title = "Shopping",
            Description = null
        };

        var categoryDto = new CategoryDto
        {
            Id = 1,
            Title = "Shopping",
            Description = null,
            TransactionsCount = 0
        };

        _mockMapper.Setup(m => m.Map<Category>(categoryCreateDto)).Returns(categoryEntity);
        _mockCategoryRepository.Setup(r => r.AddCategoryAsync(categoryEntity)).ReturnsAsync(categoryEntity);
        _mockMapper.Setup(m => m.Map<CategoryDto>(categoryEntity)).Returns(categoryDto);

        // Act
        var result = await _categoryService.AddCategoryAsync(categoryCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryDto.Title, result.Title);
        Assert.Null(result.Description);

        _mockMapper.Verify(m => m.Map<Category>(categoryCreateDto), Times.Once);
        _mockCategoryRepository.Verify(r => r.AddCategoryAsync(categoryEntity), Times.Once);
    }
}