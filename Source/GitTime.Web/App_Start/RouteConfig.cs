using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GitTime.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            // GitHub Auth

            routes.MapRoute(
                name: "GitHubLogin",
                url: "login/github",
                defaults: new { controller = "Login", action = "GitHubLogin" }
            );

            routes.MapRoute(
                name: "GitHubAuthorization",
                url: "login/github/authorization",
                defaults: new { controller = "Login", action = "GitHubAuthorization" }
            );

            // Logout

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Login", action = "Logout" }
            );

            // Default

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
