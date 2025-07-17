using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Pfm.Domain.Entities;

namespace Pfm.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasMaxLength(8).IsRequired();
            builder.Property(t => t.BeneficiaryName).HasMaxLength(50);
            builder.Property(t => t.Date).IsRequired();
            builder.Property(t => t.Direction).IsRequired();
            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.Description).HasMaxLength(100);
            builder.Property(t => t.Currency).HasMaxLength(3).IsRequired();
            builder.Property(t => t.CatCode).HasMaxLength(4);

            builder.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CatCode);
        }
    }
}
