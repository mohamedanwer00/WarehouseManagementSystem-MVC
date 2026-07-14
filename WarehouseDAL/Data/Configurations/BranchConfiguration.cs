using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Entities;

namespace WarehouseDAL.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.Address)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(b => b.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);


            builder.HasData(new Branch
            {
                Id = 1,
                Name = "الفرع الرئيسي",
                Address = "القاهرة - وسط البلد",
                PhoneNumber = "01146613992",
                CreatedOn = new DateTime(2026, 1, 1),
                UpdatedOn = new DateTime(2026, 1, 1)
            });
        }
    }
}