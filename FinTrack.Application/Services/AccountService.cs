using AutoMapper;
using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Repositories.Interfaces;

namespace FinTrack.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public AccountService(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountWithTransactionDto> AddAccountAsync(AccountCreateDto accountCreateDto)
    {
        var entity = _mapper.Map<Account>(accountCreateDto);

        var savedEntity = await _accountRepository.AddAccountAsync(entity);

        return _mapper.Map<AccountWithTransactionDto>(savedEntity);
    }

    public async Task<bool> DeleteAccountAsync(int idAccount, int idUser)
    {
        await _accountRepository.DeleteAccountAsync(idAccount, idUser);
        return true;
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync(int idUser)
    {
        var entities = await _accountRepository.GetAllAccountsAsync(idUser);

        var dtos = _mapper.Map<IEnumerable<AccountDto>>(entities);
        return dtos;
    }

    public async Task<IEnumerable<AccountWithTransactionDto>> GetAllAccountsWithTransactionsAsync(int idUser)
    {
        var entities = await _accountRepository.GetAllAccountsWithTransactionsAsync(idUser);

        var dtos = _mapper.Map<IEnumerable<AccountWithTransactionDto>>(entities);
        return dtos;
    }

    public async Task<AccountWithTransactionDto> GetAccountByIdAsync(int idAccount, int idUser)
    {
        var entity = await _accountRepository.GetAccountWithTransactionsAsync(idAccount, idUser);

        return _mapper.Map<AccountWithTransactionDto>(entity);
    }

    public async Task<AccountWithTransactionDto> UpdateAccountAsync(AccountUpdateDto accountDto)
    {
        var entity = _mapper.Map<Account>(accountDto);

        var updatedEntity = await _accountRepository.UpdateAccountAsync(entity);

        return _mapper.Map<AccountWithTransactionDto>(updatedEntity);
    }
}