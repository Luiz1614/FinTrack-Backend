using AutoMapper;
using Fintrack.Contracts.DTOs.Transaction;
using Fintrack.Contracts.Pagination;
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

    public async Task<bool> DeleteTransactionAsync(int idTransaction, int idUser)
    {
        await _transactionRepository.DeleteTransactionAsync(idTransaction, idUser);
        return true;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync(TransactionParameters transactionParameters, int idUser)
    {
        var entities = await _transactionRepository.GetAllTransactionsAsync(transactionParameters, idUser);

        return _mapper.Map<IEnumerable<TransactionDto>>(entities);
    }

    public async Task<TransactionDto> GetTransactionByIdAsync(int idTransaction, int idUser)
    {
        var entity = await _transactionRepository.GetTransactionByIdAsync(idTransaction, idUser);

        return _mapper.Map<TransactionDto>(entity);
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionByAccountAsync(int idAccount, int idUser)
    {
        var entity = await _transactionRepository.GetByAccountAsync(idAccount, idUser);

        return _mapper.Map<IEnumerable<TransactionDto>>(entity);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(TransactionUpdateDto transactionUpdateDto, int idUser)
    {
        var entity = _mapper.Map<Transaction>(transactionUpdateDto);

        var updatedEntity = await _transactionRepository.UpdateTransactionAsync(entity, idUser);

        return _mapper.Map<TransactionDto>(updatedEntity);
    }

    public async Task<IEnumerable<TransactionDto>> GetTrasactionsByMonthAsync(int idUser, int year, int month)
    {
        var entities = await _transactionRepository.GetTransactionByMonthAsync(idUser, year, month);

        return _mapper.Map<IEnumerable<TransactionDto>>(entities);
    }
}
