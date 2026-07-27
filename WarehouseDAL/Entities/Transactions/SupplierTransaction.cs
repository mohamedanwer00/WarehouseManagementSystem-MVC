namespace WarehouseDAL.Entities.Transactions;

public class SupplierTransaction : BaseEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public decimal Amount { get; set; }
    public int? PurchaseInvoiceId { get; set; }
    public PurchaseInvoice? PurchaseInvoice { get; set; }
    public SupplierTransactionType SupplierTransactionType { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
}
public enum SupplierTransactionType
{
    OPenBalance=1,
    PurchaseInvoice = 2,
    Payment = 3,// سداد
}