using Fintrack.Contracts.DTOs.Category;
using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[Authorize(Policy = "UserOnly")]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Obtém uma lista de todas as categorias.
    /// </summary>
    /// <returns>Uma lista de objetos de categoria.</returns>
    /// <response code="200">Retorna a lista de categorias.</response>
    /// <response code="404">Se nenhuma categoria for encontrada.</response>
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        if (categories == null)
            return StatusCode((int)HttpStatusCode.NotFound, $"Nenhuma categoria encontrada.");

        return Ok(categories);
    }

    /// <summary>
    /// Obtém uma categoria específica pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da categoria a ser recuperada.</param>
    /// <returns>O objeto da categoria correspondente ao ID.</returns>
    /// <response code="200">Retorna a categoria solicitada.</response>
    /// <response code="404">Se a categoria com o ID especificado não for encontrada.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma categoria encontrada para o id inserido.");

        return Ok(category);
    }

    /// <summary>
    /// Adiciona uma nova categoria. (Requer privilégios de Administrador)
    /// </summary>
    /// <param name="categoryCreateDto">Os dados para a criação da nova categoria.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="201">Indica que a categoria foi criada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDto categoryCreateDto)
    {
        var result = await _categoryService.AddCategoryAsync(categoryCreateDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a categoria. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Categoria criada com sucesso!");
    }

    /// <summary>
    /// Atualiza uma categoria existente. (Requer privilégios de Administrador)
    /// </summary>
    /// <param name="categoryUpdateDto">Os dados para a atualização da categoria.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que a categoria foi atualizada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto categoryUpdateDto)
    {
        var result = await _categoryService.UpdateCategoryAsync(categoryUpdateDto);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a categoria. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Categoria atualizada com sucesso!");
    }

    /// <summary>
    /// Exclui uma categoria pelo seu ID. (Requer privilégios de Administrador)
    /// </summary>
    /// <param name="id">O ID da categoria a ser excluída.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que a categoria foi excluída com sucesso.</response>
    /// <response code="400">Se a exclusão da categoria falhar.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (result == false)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a categoria. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Categoria deletada com sucesso!");
    }
}