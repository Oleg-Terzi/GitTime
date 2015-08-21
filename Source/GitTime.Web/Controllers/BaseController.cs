using System.Web.Mvc;

namespace GitTime.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ViewResult Error(object model, string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;

            return View(model);
        }
    }
}