using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GitTime.Web.Controllers
{
    public class GitHubController : Controller
    {
        // GET: GitHub
        public ActionResult Index()
        {
            return View();
        }
    }
}