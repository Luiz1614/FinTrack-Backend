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
        try
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            if (accounts == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma conta encontrada.");

            return Ok(accounts);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAccountByIdAsync([FromQuery] int id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);

            if (account == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma conta encontrada para o id fornecido.");

            return Ok(account);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddAccount([FromBody] AccountCreateDto account)
    {
        try
        {
            var result = await _accountService.AddAccountAsync(account);

            if (result == null)
                return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a conta. Verifique os dados enviados.");

            return StatusCode((int)HttpStatusCode.Created, "Conta criada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateDto account)
    {
        try
        {
            var result = await _accountService.UpdateAccountAsync(account);

            if (result == null)
                return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a conta. Verifique os dados enviados.");

            return StatusCode((int)HttpStatusCode.NoContent, "Conta atualizada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount([FromQuery] int id)
    {
        try
        {
            var result = await _accountService.DeleteAccountAsync(id);

            if(result == false)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a conta. Verifique os dados enviados.");
            }

            return StatusCode((int)HttpStatusCode.NoContent, "Conta deletada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }
}
