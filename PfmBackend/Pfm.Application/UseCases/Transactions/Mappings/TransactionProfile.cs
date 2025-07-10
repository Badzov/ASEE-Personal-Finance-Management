using AutoMapper;
using Pfm.Application.UseCases.Transactions.Commands.ImportTransactions;
using Pfm.Application.UseCases.Transactions.Queries.GetTransactions;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;

namespace Pfm.Application.UseCases.Transactions.Mappings
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, GetTransactionsDto>()
                .ForMember(dest => dest.Direction,
                    opt => opt.MapFrom(src => src.Direction.ToString()))
                .ForMember(dest => dest.Kind,
                    opt => opt.MapFrom(src => src.Kind.ToString()))
                .ForMember(dest => dest.Mcc,
                    opt => opt.MapFrom(src => src.Mcc.HasValue ? src.Mcc.Value.ToString() : null))
                .ForMember(dest => dest.CategoryCode,
                    opt => opt.MapFrom(src => src.CatCode));

            CreateMap<ImportTransactionsDto, Transaction>()
            .ForMember(dest => dest.Direction,
                opt => opt.MapFrom(src => src.Direction.ToLower() == "d"
                    ? TransactionDirection.Debit
                    : TransactionDirection.Credit))
            .ForMember(dest => dest.Kind,
                opt => opt.MapFrom(src => Enum.Parse<TransactionKind>(src.Kind, true)))
            .ForMember(dest => dest.Mcc,
                opt => opt.MapFrom(src => src.Mcc.HasValue ? (MccCode?)src.Mcc.Value : null))
            .ForMember(dest => dest.CatCode,
                opt => opt.Ignore()); 
        }
    }
    
}
