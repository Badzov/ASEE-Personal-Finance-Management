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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Code);
            builder.Property(c => c.Code).HasMaxLength(4).IsRequired();
            builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
            builder.Property(c => c.ParentCode).HasMaxLength(4);

            builder.HasMany(c => c.Subcategories)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentCode)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
