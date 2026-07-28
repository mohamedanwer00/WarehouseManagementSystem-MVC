using Microsoft.AspNetCore.Mvc.Rendering;
using WarehouseDAL.Entities.Transactions;

namespace WarehouseBLL.BusinessServices.View_Models.CashBox;

public class CashBoxStatementViewModel
{
    public int CashBoxId { get; set; }
    public string CashBoxName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public IEnumerable<SelectListItem> CashBoxes { get; set; } = [];

    public decimal OpeningBalanceBeforePeriod { get; set; }   // الرصيد قبل بدء الفترة
    public decimal TotalIn { get; set; }                     // مجموع الوارد (إيداع + مبيعات + رصيد افتتاحي داخل الفترة)
    public decimal TotalOut { get; set; }                    // مجموع الصادر (سحب + مشتريات)
    public decimal ClosingBalanceAfterPeriod { get; set; }   // الرصيد بعد نهاية الفترة

    public List<CashStatementLineViewModel> Lines { get; set; } = [];
    public bool HasTransactions { get; set; }
}

public class CashStatementLineViewModel
{
    public DateTime Date { get; set; }
    public CashTransactionType TransactionType { get; set; }  // نوع العملية
    public string TransactionTypeName { get; set; } = string.Empty;  // اسم العملية عربي
    public string? Notes { get; set; }                        // ملاحظات
    public string? ReferenceNumber { get; set; }              // رقم المرجع (رقم فاتورة)
    public int? ReferenceInvoiceId { get; set; }              // معرف الفاتورة للرابط
    public bool IsSalesInvoice { get; set; }                  // تحديد الرابط لفاتورة مبيعات
    public bool IsPurchaseInvoice { get; set; }               // تحديد الرابط لفاتورة مشتريات

    public decimal BalanceBefore { get; set; }                // الرصيد قبل العملية
    public decimal? InAmount { get; set; }                    // المبلغ الوارد
    public decimal? OutAmount { get; set; }                   // المبلغ الصادر
    public decimal BalanceAfter { get; set; }                 // الرصيد بعد العملية
}
