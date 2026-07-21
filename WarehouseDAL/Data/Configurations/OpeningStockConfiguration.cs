using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class OpeningStockConfiguration : IEntityTypeConfiguration<OpeningStock>
    {
        public void Configure(EntityTypeBuilder<OpeningStock> builder)
        {
            builder.Property(x => x.Quantity)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            builder.HasOne(x => x.Warehouse)
                   .WithMany(x => x.Stocks)
                   .HasForeignKey(x => x.WarehouseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.WarehouseId,
                x.ProductId
            }).IsUnique();
        }
    }
}
