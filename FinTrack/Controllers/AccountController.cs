using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();

        if (accounts == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma conta encontrada.");

        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccountByIdAsync([FromQuery] int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);

        if (account == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma conta encontrada para o id fornecido.");

        return Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> AddAccount([FromBody] AccountCreateDto account)
    {
        var result = await _accountService.AddAccountAsync(account);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a conta. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Conta criada com sucesso!");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateDto account)
    {
        var result = await _accountService.UpdateAccountAsync(account);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a conta. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Conta atualizada com sucesso!");
    }

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
