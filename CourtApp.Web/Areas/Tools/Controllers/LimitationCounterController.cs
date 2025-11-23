using Microsoft.AspNetCore.Mvc;

namespace CourtApp.Web.Areas.Tools.Controllers
{
    public class LimitationCounterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
