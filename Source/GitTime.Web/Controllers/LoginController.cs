using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GitTime.Web.Infrastructure.GitHub;
using GitTime.Web.Infrastructure.Helpers;
using GitTime.Web.Infrastructure.Web;
using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;

namespace GitTime.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        #region Constants

        private static class SessionKeys
        {
            public const String ErrorMessage = "LoginController.ErrorMessage";
        }

        #endregion

        #region Actions

        public ActionResult Index(String error)
        {
            if ((User as GitTimeUser) != null && GitTimeUser.Current.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (error == "1")
            {
                var message = (String)Session[SessionKeys.ErrorMessage];

                Session[SessionKeys.ErrorMessage] = null;

                if (!String.IsNullOrEmpty(message))
                    return Error(null, message);
            }

            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginModel model)
        {
            if (!ModelState.IsValid)
                return Error(model, "Email and password are required fields.");

            using (var db = new GitTimeContext())
            {
                var user = await GetUser(db, model.Email);
                if (user == null || !string.Equals(model.Password, user.Password))
                    return Error(model, "Incorrect email or password.");
            }

            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

            GitHubUser.Reload();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            using (var db = new GitTimeContext())
            {
                var accessTokens = await db.AccessTokens.Where(t => t.ContactID == GitTimeUser.Current.ID).ToArrayAsync();
                foreach (var accessToken in accessTokens)
                    db.AccessTokens.Remove(accessToken);

                await db.SaveChangesAsync();
            }

            Session.Abandon();

            FormsAuthentication.SignOut();

            return Redirect(FormsAuthentication.LoginUrl);
        }

        [HttpGet]
        public ActionResult GitHubLogin()
        {
            if (!GitHubHelper.IsConfigured)
                return HttpNotFound();

            var callbackUrl = HttpUtility.UrlEncode(Url.Action("GitHubAuthorization", null, null, Request.Url.Scheme));
            var authorizationUrl = GitHubHelper.GetAuthorizationUrl(callbackUrl, "user:email", "repo", "notifications");

            return Redirect(authorizationUrl);
        }

        [HttpGet]
        public async Task<ActionResult> GitHubAuthorization(String code, String state, String error, String error_description, String error_uri)
        {
            String errorMessage = null;

            if (String.IsNullOrEmpty(error))
            {
                try
                {
                    AccessToken accessToken = await GitHubHelper.RequestAccessTokenAsync(code, state);
                    if (accessToken != null)
                    {
                        errorMessage = "Your GitHub account is not registered in the system. Please contact the system administrator.";

                        var emails = await GitHubHelper.RequestUserEmailsAsync(accessToken);
                        if (emails != null)
                        {
                            var userEmail = emails.FirstOrDefault(e => e.IsPrimary) ?? emails.FirstOrDefault();
                            if (userEmail != null && !String.IsNullOrEmpty(userEmail.Email))
                            {
                                using (var db = new GitTimeContext())
                                {
                                    var user = await GetUser(db, userEmail.Email);
                                    if (user != null)
                                    {
                                        accessToken.ContactID = user.ID;

                                        await SaveAccessToken(db, accessToken);

                                        FormsAuthentication.SetAuthCookie(user.Email, false);

                                        GitHubUser.Reload();

                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (GitTimeException gtex)
                {
                    if (gtex.Message != null)
                        errorMessage = gtex.Message.Replace("\r", String.Empty).Replace("\n", "<br/>");
                }
            }
            else
            {
                errorMessage = String.Format("{0}<br/>Details: {1}", error_description, error_uri);
            }

            Session[SessionKeys.ErrorMessage] = errorMessage ?? "An error occurred during authorization through GitHub.";

            return RedirectToAction("index", new { error = 1 });
        }

        #endregion

        #region Helper methods

        private async static Task<Person> GetUser(GitTimeContext db, String email)
        {
            return await (db.Persons.Where(p => p.Email == email).Select(p => p)).AsNoTracking().FirstOrDefaultAsync();
        }

        private async static Task SaveAccessToken(GitTimeContext db, AccessToken accessToken)
        {
            var dbAccessToken = await db.AccessTokens.Where(a => a.ContactID == accessToken.ContactID && a.Application == accessToken.Application).SingleOrDefaultAsync();

            if (dbAccessToken != null)
            {
                dbAccessToken.Key = accessToken.Key;
                dbAccessToken.UtcCreated = accessToken.UtcCreated;
            }
            else
            {
                db.AccessTokens.Add(accessToken);
            }

            await db.SaveChangesAsync();
        }

        #endregion
    }
}