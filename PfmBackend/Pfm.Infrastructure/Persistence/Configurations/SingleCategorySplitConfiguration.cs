using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfm.Infrastructure.Persistence.Configurations
{
    public class SingleCategorySplitConfiguration : IEntityTypeConfiguration<SingleCategorySplit>
    {
        public void Configure(EntityTypeBuilder<SingleCategorySplit> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TransactionId).HasMaxLength(8).IsRequired();
            builder.Property(s => s.CatCode).HasMaxLength(4).IsRequired();
            builder.Property(s => s.Amount).HasColumnType("numeric(18,2)").IsRequired();

            builder.HasOne(s => s.Transaction)
                .WithMany(t => t.Splits)
                .HasForeignKey(s => s.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CatCode);
        }
    }
}
