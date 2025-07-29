using Microsoft.EntityFrameworkCore;
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
    public class PfmDbContext : DbContext
    {
        private readonly string _provider;

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SingleCategorySplit> Splits { get; set; }

        public PfmDbContext(DbContextOptions<PfmDbContext> options, IConfiguration config) : base(options) {

            _provider = config["Database:Provider"] ?? "SqlServer";

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_provider == "PostgreSql")
            {
                modelBuilder.HasPostgresExtension("uuid-ossp");
            }

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new SingleCategorySplitConfiguration());
        }
    }
}
