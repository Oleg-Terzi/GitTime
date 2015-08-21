using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;

namespace GitTime.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            if (!ModelState.IsValid)
                return Error(model, "Email and password are required fields.");

            Person user;

            using (var db = new TimeTrackerContext())
            {
                user = (
                    db.Persons
                        .Where(p => p.Email == model.Email)
                        .Select(p => p)
                    )
                    .AsNoTracking().FirstOrDefault();
            }

            if (user == null || !string.Equals(model.Password, user.Password))
                return Error(model, "Incorrect email or password.");

            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

            return RedirectToAction("Index", "Home");
        }
    }
}