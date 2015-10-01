using System;
using System.Security.Principal;
using System.Web.Mvc;

using GitTime.Web;

namespace GitTime.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Actions

        public ActionResult Index()
        {
            return RedirectToAction("Find", "Timecard");
        }

        #endregion
    }
}