using AutoMapper;
using Pfm.Application.UseCases.SpendingAnalytics.Queries;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.SpendingAnalytics.Mappings
{
    internal class SpendingAnalysisProfile : Profile
    {
        public SpendingAnalysisProfile()
        {
            CreateMap<SpendingAnalysis, SpendingAnalysisDto>();
        }
    }
}
