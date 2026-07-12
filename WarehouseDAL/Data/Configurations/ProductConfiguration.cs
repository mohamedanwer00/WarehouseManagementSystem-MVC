using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class ProductConfiguration: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.Code)
                .IsUnique();

            builder.Property(p => p.MinimumQuantity)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.MaximumQuantity)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // العلاقة مع Category
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products) 
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);// منع الحذف المتسلسل عند حذف فئة
        }
    }
}
