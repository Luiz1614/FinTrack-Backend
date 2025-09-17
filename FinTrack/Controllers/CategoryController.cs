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
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            if (categories == null)
                return StatusCode((int) HttpStatusCode.NotFound, $"Nenhuma categoria encontrada.");

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoryById([FromHeader] int id)
    {
        try
        {
            var category = _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma categoria encontrada para o id inserido.");

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }
}
