namespace WarehouseDAL.Entities.Transactions;

public class CustomerTransaction : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public decimal Amount { get; set; }
    public CustomerTransactionType CustomerTransactionType { get; set; }
    public int? SalesInvoiceId { get; set; }
    public SalesInvoice? SalesInvoice { get; set; }
    public DateTime Date {  get; set; }
    public string? Notes { get; set; }
}
public enum CustomerTransactionType
{
    OPenBalance = 1,
    SalesInvoice = 2,
    Payment = 3,// سداد
}
