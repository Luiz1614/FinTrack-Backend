using AutoMapper;
using Fintrack.Contracts.DTOs.Transaction;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Repositories.Interfaces;

namespace FinTrack.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
    {
        this._transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<TransactionDto> AddTransactionAsync(TransactionCreateDto transactionCreateDto)
    {
        var entity = _mapper.Map<Transaction>(transactionCreateDto);

        var createdEntity = await _transactionRepository.AddTransactionAsync(entity);

        return _mapper.Map<TransactionDto>(createdEntity);
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        await _transactionRepository.DeleteTransactionAsync(id);
        return true;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        var entities = await _transactionRepository.GetAllTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(entities);
    }

    public async Task<TransactionDto> GetTransactionByIdAsync(int id)
    {
        var entity = await _transactionRepository.GetByAccountAsync(id);

        return _mapper.Map<TransactionDto>(entity);
    }

    public async Task<TransactionDto> GetTransactionByAccountAsync(int accountId)
    {
        var entity = await _transactionRepository.GetByAccountAsync(accountId);

        return _mapper.Map<TransactionDto>(entity);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(TransactionUpdateDto transactionUpdateDto)
    {
        var entity = _mapper.Map<Transaction>(transactionUpdateDto);

        var updatedEntity = await _transactionRepository.UpdateTransactionAsync(entity);

        return _mapper.Map<TransactionDto>(updatedEntity);
    }

    public async Task<IEnumerable<TransactionDto>> GetTrasactionsByMonthAsync(int idUser, int year, int month)
    {
        var entities = await _transactionRepository.GetTransactionByMonthAsync(idUser, year, month);

        return _mapper.Map<IEnumerable<TransactionDto>>(entities);
    }
}
