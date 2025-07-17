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
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Splits,
                    opt => opt.MapFrom(src => src.Splits));

            CreateMap<ImportTransactionsDto, Transaction>()

            .ForMember(dest => dest.Direction,
                opt => opt.MapFrom(src => src.Direction.ToLower() == "d"
                    ? DirectionsEnum.Debit
                    : DirectionsEnum.Credit))
            .ForMember(dest => dest.Kind,
                opt => opt.MapFrom(src => Enum.Parse<TransactionKindsEnum>(src.Kind, true)))
            .ForMember(dest => dest.Mcc,
                opt => opt.MapFrom(src => src.Mcc.HasValue ? (MccCodeEnum?)src.Mcc.Value : null))
            .ForMember(dest => dest.CatCode,
                opt => opt.Ignore()); 
        }
    }
    
}
