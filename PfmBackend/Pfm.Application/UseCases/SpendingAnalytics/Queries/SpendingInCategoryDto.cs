﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Application.UseCases.SpendingAnalytics.Queries
{
    public record SpendingInCategoryDto(
        string CatCode,
        double Amount,
        int Count
    );
}
