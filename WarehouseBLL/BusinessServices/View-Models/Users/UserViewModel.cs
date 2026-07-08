

namespace WarehouseBLL.BusinessServices.View_Models.Users
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string LastAction { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
