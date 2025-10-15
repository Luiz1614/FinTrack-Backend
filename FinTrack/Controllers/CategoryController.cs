using Fintrack.Contracts.DTOs.Category;
using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        if (categories == null)
            return StatusCode((int)HttpStatusCode.NotFound, $"Nenhuma categoria encontrada.");

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById([FromQuery] int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma categoria encontrada para o id inserido.");

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDto categoryCreateDto)
    {
        var result = await _categoryService.AddCategoryAsync(categoryCreateDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a categoria. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Categoria criada com sucesso!");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto categoryUpdateDto)
    {
        var result = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a categoria. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Categoria atualizada com sucesso!");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromQuery]int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (result == false)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a categoria. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Categoria deletada com sucesso!");
    }
}
