namespace WarehouseDAL.Entities;

public class CustomerTransaction : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public decimal Amount { get; set; }

    public BalanceType BalanceType { get; set; }
    public int? SalesInvoiceId { get; set; }
    public SalesInvoice? SalesInvoice { get; set; }
    public DateTime Date {  get; set; }
    public string? Notes { get; set; }
}
