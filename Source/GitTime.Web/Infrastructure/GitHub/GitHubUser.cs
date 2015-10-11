using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using GitTime.Web.Infrastructure.Web;
using GitTime.Web.Models;

namespace GitTime.Web.Infrastructure.GitHub
{
    public class GitHubUser
    {
        #region Constants

        private const String CurrentSessionKey = "GitTime.Web.Infrastructure.GitHub.Current";

        #endregion

        #region Properties

        public Int32 ID { get; private set; }
        public String Name { get; private set; }
        public String Email { get; private set; }
        public String AvatarUrl { get; private set; }
        public Boolean IsAuthenticated { get; private set; }

        public GitTime.Web.Models.Database.AccessToken AccessToken { get; private set; }

        #endregion

        #region Construction

        private GitHubUser()
        {
            ID = -1;
            IsAuthenticated = false;
        }

        #endregion
                
        #region Public methods

        public static GitHubUser GetCurrent()
        {
            var context = HttpContext.Current;
            var user = (GitHubUser)context.Session[CurrentSessionKey];

            if (user == null)
            {
                var userId = GitTimeUser.Current.ID;

                context.Session[CurrentSessionKey] = user = Task.Run(() => LoadUserAsync(userId)).Result;
            }

            return user;
        }

        public async static Task<GitHubUser> GetCurrentAsync()
        {
            var context = HttpContext.Current;
            var user = (GitHubUser)context.Session[CurrentSessionKey];

            if (user == null)
                context.Session[CurrentSessionKey] = user = await LoadUserAsync(GitTimeUser.Current.ID);

            return user;
        }

        public static void Reload()
        {
            HttpContext.Current.Session[CurrentSessionKey] = null;
        }

        #endregion

        #region Helper methods

        private async static Task<GitHubUser> LoadUserAsync(Int32 userId)
        {
            var result = new GitHubUser();

            using (var db = new GitTimeContext())
            {
                var token = await db.AccessTokens.Where(t => t.ContactID == userId && t.Application == "GitHub").SingleOrDefaultAsync();
                if (token != null)
                {
                    var userInfo = await GitHubHelper.RequestUserInfoAsync(token);
                    if (userInfo != null)
                    {
                        result.ID = userInfo.ID;
                        result.Name = userInfo.LoginName;
                        result.Email = userInfo.Email;
                        result.AvatarUrl = userInfo.AvatarUrl;
                        result.IsAuthenticated = true;
                        result.AccessToken = token;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}