using Fintrack.Contracts.DTOs.Transaction;
using Fintrack.Contracts.Pagination;
using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[Authorize(Policy = "UserOnly")]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Obtém uma lista de transações com base nos parâmetros de paginação.
    /// </summary>
    /// <param name="transactionParameters">Os parâmetros para paginação e filtragem das transações.</param>
    /// <returns>Uma lista paginada de objetos de transação.</returns>
    /// <response code="200">Retorna a lista de transações.</response>
    /// <response code="404">Se nenhuma transação for encontrada.</response>
    [HttpGet]
    public async Task<IActionResult> GetAllTransactions([FromQuery] TransactionParameters transactionParameters)
    {
        var transactions = await _transactionService.GetAllTransactionsAsync(transactionParameters);

        if (transactions == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma trasação encontrada.");

        return Ok(transactions);
    }

    /// <summary>
    /// Obtém uma transação específica pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da transação a ser recuperada.</param>
    /// <returns>O objeto da transação correspondente ao ID.</returns>
    /// <response code="200">Retorna a transação solicitada.</response>
    /// <response code="404">Se a transação com o ID especificado não for encontrada.</response>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma transação encontrada para o id fornecido.");

        return Ok(transaction);
    }

    /// <summary>
    /// Obtém todas as transações de uma conta específica.
    /// </summary>
    /// <param name="accountId">O ID da conta.</param>
    /// <returns>Uma lista de transações para a conta especificada.</returns>
    /// <response code="200">Retorna a lista de transações.</response>
    /// <response code="404">Se nenhuma transação for encontrada para a conta fornecida.</response>
    [HttpGet("Account/{idAccount:int}")]
    public async Task<IActionResult> GetTransactionByAccount(int accountId)
    {
        var transaction = await _transactionService.GetTransactionByAccountAsync(accountId);

        if (transaction == null)
            return StatusCode((int)HttpStatusCode.NotFound, "Nenhuma transação encontrada para a conta fornecida.");

        return Ok(transaction);
    }

    /// <summary>
    /// Obtém as transações de um usuário para um mês e ano específicos.
    /// </summary>
    /// <param name="idUser">O ID do usuário.</param>
    /// <param name="year">O ano do relatório.</param>
    /// <param name="month">O mês do relatório.</param>
    /// <returns>Uma lista de transações para o período especificado.</returns>
    /// <response code="200">Retorna a lista de transações.</response>
    /// <response code="404">Se nenhuma transação for encontrada para o período informado.</response>
    [HttpGet("Month")]
    public async Task<IActionResult> GetTransactionsByMonth([FromQuery] int idUser, int year, int month)
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

    /// <summary>
    /// Adiciona uma nova transação.
    /// </summary>
    /// <param name="transaction">Os dados para a criação da nova transação.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="201">Indica que a transação foi criada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> AddTransaction([FromBody] TransactionCreateDto transaction)
    {
        var result = await _transactionService.AddTransactionAsync(transaction);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível criar a transação. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.Created, "Transação criada com sucesso!");
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <param name="transaction">Os dados para a atualização da transação.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que a transação foi atualizada com sucesso.</response>
    /// <response code="400">Se os dados fornecidos forem inválidos.</response>
    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] TransactionUpdateDto transaction)
    {
        var result = await _transactionService.UpdateTransactionAsync(transaction);

        if (result == null)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível atualizar a transação. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Transação atualizada com sucesso!");
    }

    /// <summary>
    /// Exclui uma transação pelo seu ID.
    /// </summary>
    /// <param name="id">O ID da transação a ser excluída.</param>
    /// <returns>Status da operação.</returns>
    /// <response code="204">Indica que a transação foi excluída com sucesso.</response>
    /// <response code="400">Se a exclusão da transação falhar.</response>
    [HttpDelete]
    public async Task<IActionResult> DeleteTransaction([FromQuery] int id)
    {
        var result = await _transactionService.DeleteTransactionAsync(id);

        if (result == false)
            return StatusCode((int)HttpStatusCode.BadRequest, "Não foi possível deletar a transação. Verifique os dados enviados.");

        return StatusCode((int)HttpStatusCode.NoContent, "Transação deletada com sucesso!");
    }
}