using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => c.PhoneNumber)
                .IsUnique();

            builder.Property(c => c.OpeningBalance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.CurrentBalance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.CustomerType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.OpeningBalanceType)
                .IsRequired()
                .HasConversion<int>();
        }
    }
}
