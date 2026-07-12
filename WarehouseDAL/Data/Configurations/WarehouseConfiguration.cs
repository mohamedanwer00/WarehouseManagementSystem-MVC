using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(w => w.Address)
                .IsRequired()
                .HasMaxLength(250);

            // العلاقة مع الفرع (Branch)
            builder.HasOne(w => w.Branch)
                .WithMany(b => b.Warehouses)
                .HasForeignKey(w => w.BranchId)
                .OnDelete(DeleteBehavior.Restrict); // حماية لمنع حذف الفرع إذا كان يحتوي على مخازن
        }
    }
}
