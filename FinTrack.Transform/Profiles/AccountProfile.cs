using AutoMapper;
using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;
using FinTrack.Domain.Entities;

namespace FinTrack.Transform.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        // Domain -> DTO
        CreateMap<Account, AccountDto>()
            .ForMember(d => d.CurrentBalance, opt => opt.Ignore()) // calculado em serviço
            .ForMember(d => d.Transactions, opt => opt.MapFrom(s => s.Transactions));

        CreateMap<Transaction, AccountTransactionDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.CategoryTitle, opt => opt.MapFrom(s => s.Category != null ? s.Category.Title : string.Empty));

        // DTO -> Domain (Create)
        CreateMap<AccountCreateDto, Account>();

        // DTO -> Domain (Update)
        CreateMap<AccountUpdateDto, Account>()
            .ForMember(d => d.Transactions, opt => opt.Ignore());
    }
}
