using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class SupplierConfiguration: IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Address)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(s => s.OpeningBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0.00m);

            builder.Property(s => s.CurrentBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0.00m);

            builder.Property(s => s.OpeningBalanceType)
                .IsRequired()
                .HasMaxLength(15)
                .HasConversion<string>();

            builder.Property(s => s.LastAction).HasMaxLength(50);
        }
    }
}
