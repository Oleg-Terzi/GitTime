using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace GitTime.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        #region Security

        protected virtual void ApplySecurity(AuthenticationContext context)
        {
            if (!CanView(context.Principal))
                context.Result = new ViewResult { ViewName = "_AccessDenied" };
        }

        protected virtual Boolean CanView(IPrincipal principal)
        {
            return true;
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            if (filterContext.Result == null)
                ApplySecurity(filterContext);
        }

        #endregion

        #region Helper methods

        protected ViewResult Error(Object model, String errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;

            return View(model);
        }

        #endregion
    }
}