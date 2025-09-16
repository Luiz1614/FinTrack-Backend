using AutoMapper;
using Fintrack.Contracts.DTOs.Transaction;
using FinTrack.Domain.Entities;

namespace FinTrack.Transform.Profiles;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        // Domain -> DTO
        CreateMap<Transaction, TransactionDto>()
            .ForMember(d => d.CategoryTitle, opt => opt.MapFrom(s => s.Category != null ? s.Category.Title : string.Empty))
            .ForMember(d => d.AccountName, opt => opt.MapFrom(s => s.Account != null ? s.Account.Name : string.Empty));

        // DTO -> Domain (Create)
        CreateMap<TransactionCreateDto, Transaction>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.Account, opt => opt.Ignore());

        // DTO -> Domain (Update)
        CreateMap<TransactionUpdateDto, Transaction>()
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.Account, opt => opt.Ignore())
            // Só atualiza Title se veio não-nulo
            .ForMember(d => d.Title, opt =>
            {
                opt.PreCondition(src => src.Title != null);
                opt.MapFrom(src => src.Title!);
            });
    }
}
