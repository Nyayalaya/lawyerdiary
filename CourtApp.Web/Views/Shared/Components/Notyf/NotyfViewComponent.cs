using Microsoft.AspNetCore.Mvc;

namespace CourtApp.Web.Views.Shared.Components.Notyf
{
    public class NotyfViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
