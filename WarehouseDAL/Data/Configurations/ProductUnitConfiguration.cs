using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class ProductUnitConfiguration : IEntityTypeConfiguration<ProductUnit>
    {
        public void Configure(EntityTypeBuilder<ProductUnit> builder)
        {
            builder.HasKey(pu => pu.Id);

            // العلاقة مع Product
            builder.HasOne(pu => pu.Product)
                .WithMany(p => p.ProductUnits)
                .HasForeignKey(pu => pu.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة مع Unit
            builder.HasOne(pu => pu.Unit)
                .WithMany(u => u.ProductUnits)
                .HasForeignKey(pu => pu.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(pu => pu.Factor)
                .IsRequired();

            builder.Property(pu => pu.IsBaseUnit)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pu => pu.PurchasePrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(pu => pu.SellingPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // منع تكرار نفس الوحدة لنفس المنتج
            builder.HasIndex(pu => new { pu.ProductId, pu.UnitId })
                .IsUnique();
        }
    }
}
