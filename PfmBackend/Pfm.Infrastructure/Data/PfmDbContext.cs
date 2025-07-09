using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Data
{
    public class PfmDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Split> Splits { get; set; }

        public PfmDbContext(DbContextOptions<PfmDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Subcategories)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Splits)
                .WithOne(s => s.Transaction)
                .HasForeignKey(s => s.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
