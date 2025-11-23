using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Litigation.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CourtApp.Web.Areas.Tools.Controllers
{
    [Area("Tools")]
    public class CourtFeeController : BaseController<CourtFeeController>
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
