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

    public async Task<AccountDto> AddAccountAsync(AccountCreateDto accountCreateDto)
    {
        var entity = _mapper.Map<Account>(accountCreateDto);

        var savedEntity = await _accountRepository.AddAccountAsync(entity);

        return _mapper.Map<AccountDto>(savedEntity);
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        await _accountRepository.DeleteAccountAsync(id);
        return true;
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
    {
        var entities = await _accountRepository.GetAllAccountsAsync();

        return _mapper.Map<IEnumerable<AccountDto>>(entities);
    }

    public async Task<AccountDto> GetAccountByIdAsync(int id)
    {
        var entity = await _accountRepository.GetAccountByIdAsync(id);

        return _mapper.Map<AccountDto>(entity);
    }

    public async Task<AccountDto> UpdateAccountAsync(AccountUpdateDto accountDto)
    {
        var entity = _mapper.Map<Account>(accountDto);

        var updatedEntity = await _accountRepository.UpdateAccountAsync(entity);

        return _mapper.Map<AccountDto>(updatedEntity);
    }
}
