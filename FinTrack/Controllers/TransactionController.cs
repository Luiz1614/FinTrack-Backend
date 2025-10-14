using Fintrack.Contracts.DTOs.Transaction;
using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        try
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();

            if (transactions == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma trasação encontrada.");

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTransactionById([FromQuery] int id)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma transação encontrada para o id fornecido.");

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet("Account/{idAccount:int}")]
    public async Task<IActionResult> GetTransactionByAccount([FromQuery] int accountId)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByAccountAsync(accountId);

            if (transaction == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma transação encontrada para a conta fornecida.");

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet("Month")]
    public async Task<IActionResult> GetTransactionsByMonth([FromQuery]int year, int month)
    {
        try
        {
            var transactions = await _transactionService.GetTrasactionsByMonthAsync(year, month);

            if (transactions == null)
                return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma trasação encontrada para o mês informado.");

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }


    [HttpPost]
    public async Task<IActionResult> AddTransaction([FromBody] TransactionCreateDto transaction)
    {
        try
        {
            var result = await _transactionService.AddTransactionAsync(transaction);

            if (result == null)
                return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a transação. Verifique os dados enviados.");

            return StatusCode((int)HttpStatusCode.Created, "Transação criada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] TransactionUpdateDto transaction)
    {
        try
        {
            var result = await _transactionService.UpdateTransactionAsync(transaction);

            if (result == null)
                return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a transação. Verifique os dados enviados.");

            return StatusCode((int)HttpStatusCode.NoContent, "Transação atualizada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTransaction([FromQuery] int id)
    {
        try
        {
            var result = await _transactionService.DeleteTransactionAsync(id);

            if (result == false)
                return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a transação. Verifique os dados enviados.");

            return StatusCode((int)HttpStatusCode.NoContent, "Transação deletada com sucesso!");
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }
}
