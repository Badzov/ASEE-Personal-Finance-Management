using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Pfm.Domain.Entities;
using Pfm.Infrastructure.Persistence.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Persistence.DbContexts
{
    public class PfmSqlServerDbContext : PfmDbContext
    {
        public PfmSqlServerDbContext(DbContextOptions<PfmSqlServerDbContext> options) : base(options) { }
    }
}
