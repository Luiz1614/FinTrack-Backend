using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace FinTrack.Controllers;

[Authorize(Policy = "UserOnly")]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Obtém uma lista de todas as contas do usuário autenticado.
    /// </summary>
    /// <returns>Uma lista de objetos de conta.</returns>
    /// <response code="200">Retorna a lista de contas.</response>
    /// <response code="404">Se nenhuma conta for encontrada.</response>
    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();

        if (accounts == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma conta encontrada.");

        return Ok(accounts);
    }

    /// <summary>
    /// Obtém uma conta específica pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da conta a ser recuperada.</param>
    /// <returns>O objeto da conta correspondente ao ID.</returns>
    /// <response code="200">Retorna a conta solicitada.</response>
    /// <response code="404">Se a conta com o ID especificado não for encontrada.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccountByIdAsync(int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);

        if (account == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma conta encontrada para o id fornecido.");

        return Ok(account);
    }

    /// <summary>
    /// Adiciona uma nova conta para o usuário autenticado.
    /// </summary>
    /// <param name="account">Os dados para a criação da nova conta.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="201">Indica que a conta foi criada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    /// <response code="500">Se ocorrer um erro ao identificar o usuário.</response>
    [HttpPost]
    public async Task<IActionResult> AddAccount([FromBody] AccountCreateDto account)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var accountName = User.FindFirstValue(ClaimTypes.Name);

        if (!int.TryParse(userIdValue, out var userId))
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Usuário não encontrado");

        account.UserId = userId;
        account.Name = accountName;

        var result = await _accountService.AddAccountAsync(account);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a conta. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Conta criada com sucesso!");
    }

    /// <summary>
    /// Atualiza uma conta existente.
    /// </summary>
    /// <param name="account">Os dados para a atualização da conta.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que a conta foi atualizada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPut]
    public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateDto account)
    {
        var result = await _accountService.UpdateAccountAsync(account);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a conta. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Conta atualizada com sucesso!");
    }

    /// <summary>
    /// Exclui uma conta pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da conta a ser excluída.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que a conta foi excluída com sucesso.</response>
    /// <response code="400">Se a exclusão da conta falhar.</response>
    [HttpDelete]
    public async Task<IActionResult> DeleteAccount([FromQuery] int id)
    {
        var result = await _accountService.DeleteAccountAsync(id);

        if (result == false)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a conta. Verifique os dados enviados.");
        }

        return StatusCode((int)HttpStatusCode.NoContent, "Conta deletada com sucesso!");
    }
}