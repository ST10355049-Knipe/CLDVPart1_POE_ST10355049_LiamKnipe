using Microsoft.AspNetCore.Mvc;

namespace ST10355049.Models
{
    public class Cart : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
