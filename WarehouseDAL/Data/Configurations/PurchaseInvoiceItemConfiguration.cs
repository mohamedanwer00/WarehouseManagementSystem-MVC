using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations;

public class PurchaseInvoiceItemConfiguration : IEntityTypeConfiguration<PurchaseInvoiceItem>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoiceItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PurchasePrice)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TotalPrice)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Discount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProductUnit)
               .WithMany()
               .HasForeignKey(x => x.ProductUnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}