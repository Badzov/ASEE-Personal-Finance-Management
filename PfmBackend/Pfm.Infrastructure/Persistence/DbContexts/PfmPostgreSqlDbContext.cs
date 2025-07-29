using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using Pfm.Infrastructure.Persistence.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Persistence.DbContexts
{
    public class PfmPostgreSqlDbContext : PfmDbContext
    {
        public PfmPostgreSqlDbContext(DbContextOptions<PfmPostgreSqlDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");
        }
    }
}
