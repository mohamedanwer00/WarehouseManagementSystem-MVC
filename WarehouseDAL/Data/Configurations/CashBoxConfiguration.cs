using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class CashBoxConfiguration : IEntityTypeConfiguration<CashBox>
    {
        public void Configure(EntityTypeBuilder<CashBox> builder)
        {
            builder.HasKey(cb => cb.Id);

            builder.Property(cb => cb.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(cb => cb.OpeningBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0.00);

            builder.Property(cb => cb.CurrentBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0.00);


            // العلاقة مع الفرع (Branch)
            builder.HasOne(cb => cb.Branch)
                .WithMany(b => b.CashBoxes)
                .HasForeignKey(cb => cb.BranchId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}