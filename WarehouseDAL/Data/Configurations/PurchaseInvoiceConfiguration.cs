using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseDAL.Entities;

namespace WarehouseDAL.Data.Configurations;

public class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.ToTable("PurchaseInvoices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InvoiceNumber)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(x => x.TotalAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Discount)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Paid)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Remaining)
               .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Notes)
               .HasMaxLength(500);

        builder.HasOne(x => x.Supplier)
               .WithMany(x => x.PurchaseInvoices)
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Branch)
               .WithMany(x => x.PurchaseInvoices)
               .HasForeignKey(x => x.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
               .WithMany()
               .HasForeignKey(x => x.WarehouseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PurchaseInvoiceItems)
               .WithOne(x => x.PurchaseInvoice)
               .HasForeignKey(x => x.PurchaseInvoiceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}