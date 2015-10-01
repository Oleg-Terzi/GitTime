using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace GitTime.Web.Infrastructure.Web
{
    public class GitTimeAuthenticationModule : IHttpModule
    {
        #region Properties

        private static AuthenticationMode AuthenticationMode
        {
            get
            {
                var authentication = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");
                return authentication.Mode;
            }
        }

        #endregion

        #region Initialization and disposal

        public void Init(HttpApplication context)
        {
            var authenticateRequestHelper = new EventHandlerTaskAsyncHelper(OnAuthenticateRequest);
            context.AddOnAuthenticateRequestAsync(authenticateRequestHelper.BeginEventHandler, authenticateRequestHelper.EndEventHandler);
        }

        public void Dispose()
        {

        }

        #endregion

        #region Event handlers

        private async Task OnAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            FormsAuthenticationTicket authTicket = GetAuthTicket(context.Request.Cookies);
            if (authTicket == null || authTicket.Expired)
                return;

            context.User = await GitTimeUser.Open(authTicket.Name);
        }

        #endregion

        #region Helper methods

        private static FormsAuthenticationTicket GetAuthTicket(HttpCookieCollection cookies)
        {
            if (cookies.Count == 0)
                return null;

            HttpCookie authCookie = cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || String.IsNullOrEmpty(authCookie.Value))
                return null;

            try
            {
                return FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (HttpException)
            {
                return null;
            }
        }

        #endregion
    }
}