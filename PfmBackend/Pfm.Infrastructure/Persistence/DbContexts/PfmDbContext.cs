using Microsoft.EntityFrameworkCore;
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
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SingleCategorySplit> Splits { get; set; }

        public PfmDbContext(DbContextOptions<PfmDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new SingleCategorySplitConfiguration());
        }
    }
}
