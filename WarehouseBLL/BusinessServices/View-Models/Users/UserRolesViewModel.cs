namespace WarehouseBLL.BusinessServices.View_Models.Users
{
    public class UserRolesViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<CheckBoxViewModel> Roles { get; set; }
    }
}
