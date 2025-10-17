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
        var transactions = await _transactionService.GetAllTransactionsAsync();

        if (transactions == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma trasação encontrada.");

        return Ok(transactions);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTransactionById([FromQuery] int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma transação encontrada para o id fornecido.");

        return Ok(transaction);
    }

    [HttpGet("Account/{idAccount:int}")]
    public async Task<IActionResult> GetTransactionByAccount([FromQuery] int accountId)
    {
        var transaction = await _transactionService.GetTransactionByAccountAsync(accountId);

        if (transaction == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma transação encontrada para a conta fornecida.");

        return Ok(transaction);
    }

    [HttpGet("Month")]
    public async Task<IActionResult> GetTransactionsByMonth([FromQuery]int idUser, int year, int month)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "O mês precisa se entre 1 e 12");
        }

        var transactions = await _transactionService.GetTrasactionsByMonthAsync(idUser, year, month);

        if (transactions == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma trasação encontrada para o mês informado.");

        return Ok(transactions);
    }


    [HttpPost]
    public async Task<IActionResult> AddTransaction([FromBody] TransactionCreateDto transaction)
    {
        var result = await _transactionService.AddTransactionAsync(transaction);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a transação. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Transação criada com sucesso!");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] TransactionUpdateDto transaction)
    {
        var result = await _transactionService.UpdateTransactionAsync(transaction);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a transação. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Transação atualizada com sucesso!");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTransaction([FromQuery] int id)
    {
        var result = await _transactionService.DeleteTransactionAsync(id);

        if (result == false)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a transação. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Transação deletada com sucesso!");
    }
}
