using Microsoft.AspNetCore.Mvc;

namespace WarehousePL.Web.Controllers.Categories
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
