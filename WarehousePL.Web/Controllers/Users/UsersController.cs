using Microsoft.AspNetCore.Mvc;

namespace WarehousePL.Web.Controllers.Users
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
