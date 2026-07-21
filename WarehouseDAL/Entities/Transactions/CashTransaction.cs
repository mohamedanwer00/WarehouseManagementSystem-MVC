namespace WarehouseDAL.Entities.Transactions;

public class CashTransaction
{
    public int CashBoxId { get; set; }
    public CashBox CashBox { get; set; } = null!;

    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }

    public CashTransactionType TransactionType { get; set; }

    public string? Notes { get; set; }
    public DateTime Date { get; set; }
}
public enum CashTransactionType
{
    Deposit = 1,//ايداع
    Withdraw = 2,//سحب 
    Sales = 3,//بيع
    OpeningBalance = 4,//رصيد افتتاحى 
 
}
