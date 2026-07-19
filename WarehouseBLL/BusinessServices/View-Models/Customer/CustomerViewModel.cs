using WarehouseDAL.Entities.Enums;

namespace WarehouseBLL.BusinessServices.View_Models.Customer
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public CustomerType CustomerType { get; set; }
        public BalanceType OpeningBalanceType { get; set; }
        public string LastAction { get; set; } = null!;
    }
}
