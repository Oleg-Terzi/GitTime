using System.Web.Mvc;

namespace GitTime.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Find", "Timecard");
        }
    }
}