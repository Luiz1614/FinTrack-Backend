using AutoMapper;
using Fintrack.Contracts.DTOs.Transaction;
using Fintrack.Contracts.Pagination;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infraestructure.Repositories.Interfaces;

namespace FinTrack.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _accountRepository = accountRepository;
    }

    public async Task<TransactionDto> AddTransactionAsync(TransactionCreateDto transactionCreateDto, int idUser)
    {
        var entity = _mapper.Map<Transaction>(transactionCreateDto);

        var createdEntity = await _transactionRepository.AddTransactionAsync(entity);

        var account = await _accountRepository.GetAccountByIdAsync(createdEntity.AccountId, idUser);

        if (account is not null)
        {
            decimal newBalance;

            if (createdEntity.Type == TransactionType.Income)
                newBalance = account.CurrentBalance + createdEntity.Amount;
            else
                newBalance = account.CurrentBalance - createdEntity.Amount;

            await _accountRepository.UpdateCurrentBalanceAsync(account.Id, newBalance);
        }

        return _mapper.Map<TransactionDto>(createdEntity);
    }

    public async Task<bool> DeleteTransactionAsync(int idTransaction, int idUser)
    {
        var transaction = await _transactionRepository.GetTransactionByIdAsync(idTransaction, idUser);

        if (transaction is null)
            return false;

        var account = await _accountRepository.GetAccountByIdAsync(transaction.AccountId, idUser);

        if (account is not null)
        {
            decimal newBalance;

            if (transaction.Type == TransactionType.Income)
                newBalance = account.CurrentBalance - transaction.Amount;
            else
                newBalance = account.CurrentBalance + transaction.Amount;

            await _accountRepository.UpdateCurrentBalanceAsync(account.Id, newBalance);
        }

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
        var oldTransaction = await _transactionRepository.GetTransactionByIdAsync(transactionUpdateDto.Id, idUser);

        var entity = _mapper.Map<Transaction>(transactionUpdateDto);

        var updatedEntity = await _transactionRepository.UpdateTransactionAsync(entity, idUser);

        if (oldTransaction is not null)
        {
            var account = await _accountRepository.GetAccountByIdAsync(oldTransaction.AccountId, idUser);

            if (account is not null)
            {
                decimal balanceAfterRevert;

                if (oldTransaction.Type == TransactionType.Income)
                    balanceAfterRevert = account.CurrentBalance - oldTransaction.Amount;
                else
                    balanceAfterRevert = account.CurrentBalance + oldTransaction.Amount;

                decimal newBalance;

                if (entity.Type == TransactionType.Income)
                    newBalance = balanceAfterRevert + entity.Amount;
                else
                    newBalance = balanceAfterRevert - entity.Amount;

                await _accountRepository.UpdateCurrentBalanceAsync(account.Id, newBalance);
            }
        }

        return _mapper.Map<TransactionDto>(updatedEntity);
    }

    public async Task<IEnumerable<TransactionDto>> GetTrasactionsByMonthAsync(int idUser, int year, int month)
    {
        var entities = await _transactionRepository.GetTransactionByMonthAsync(idUser, year, month);

        return _mapper.Map<IEnumerable<TransactionDto>>(entities);
    }
}