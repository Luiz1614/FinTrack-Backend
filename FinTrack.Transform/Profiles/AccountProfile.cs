using AutoMapper;
using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using System.Linq;

namespace FinTrack.Transform.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        // Domain -> DTO
        CreateMap<Account, AccountDto>()
            .ForMember(d => d.CurrentBalance, opt => opt.MapFrom(s =>
                s.Transactions != null
                    ? s.InitialBalance + s.Transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount)
                    : s.InitialBalance))
            .ForMember(d => d.Transactions, opt => opt.MapFrom(s => s.Transactions));

        CreateMap<Transaction, AccountTransactionDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.CategoryTitle, opt => opt.MapFrom(s => s.Category != null ? s.Category.Title : string.Empty));

        // DTO -> Domain (Create) — fix typo and set balances
        CreateMap<AccountCreateDto, Account>()
            .ForMember(d => d.InitialBalance, opt => opt.MapFrom(s => s.InitalBalance))
            .ForMember(d => d.CurrentBalance, opt => opt.MapFrom(s => s.InitalBalance));

        // DTO -> Domain (Update)
        CreateMap<AccountUpdateDto, Account>()
            .ForMember(d => d.Transactions, opt => opt.Ignore());
    }
}