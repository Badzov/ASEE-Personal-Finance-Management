using AutoMapper;
using Pfm.Application.DTOs.Requests;
using Pfm.Domain.Entities;
using Pfm.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.Mappings.Transactions
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionCsvRequest, Transaction>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src =>
                    src.Direction.ToLower() == "d" ? TransactionDirection.Debit : TransactionDirection.Credit))
                .ForMember(dest => dest.Kind, opt => opt.MapFrom(src =>
                    Enum.Parse<TransactionKind>(src.Kind, true)))
                .ForMember(dest => dest.Mcc, opt => opt.MapFrom(src =>
                    src.Mcc.HasValue ? (MccCode?)src.Mcc.Value : null));
        }
    }
}
