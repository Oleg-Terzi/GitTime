using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

using GitTime.Web.Infrastructure.GitHub.Data;
using GitTime.Web.Infrastructure.Helpers;
using GitTime.Web.Infrastructure.Text;
using GitTime.Web.Models.Database;

namespace GitTime.Web.Infrastructure.GitHub
{
    public static class GitHubHelper
    {
        #region Classes

        private class PostParameter : Tuple<String, Object>
        {
            public PostParameter(String name, Object value)
                : base(name, value)
            {

            }
        }

        private class JsonMessage
        {
            public String message { get; set; }
            public String documentation_url { get; set; }
        }

        private class JsonAccessToken
        {
            public String access_token { get; set; }
            public String token_type { get; set; }
            public String scope { get; set; }
            public String error { get; set; }
            public String error_description { get; set; }
            public String error_uri { get; set; }
        }

        private class JsonUserEmail
        {
            public String email { get; set; }
            public Boolean primary { get; set; }
            public Boolean verified { get; set; }
        }

        private class JsonUser
        {
            public Int32 id
            {
                get { return _id ?? 0; }
                set { _id = value; }
            }

            public String login { get; set; }
            public String avatar_url { get; set; }
            public String gravatar_id { get; set; }
            public String url { get; set; }
            public String html_url { get; set; }
            public String followers_url { get; set; }
            public String following_url { get; set; }
            public String gists_url { get; set; }
            public String starred_url { get; set; }
            public String subscriptions_url { get; set; }
            public String organizations_url { get; set; }
            public String repos_url { get; set; }
            public String events_url { get; set; }
            public String received_events_url { get; set; }
            public String type { get; set; }
            public Boolean site_admin { get; set; }
            public String name { get; set; }
            public String company { get; set; }
            public String blog { get; set; }
            public String location { get; set; }
            public String email { get; set; }
            public Boolean? hireable { get; set; }
            public String bio { get; set; }
            public Int32 public_repos { get; set; }
            public Int32 public_gists { get; set; }
            public Int32 followers { get; set; }
            public Int32 following { get; set; }
            public DateTime created_at { get; set; }
            public DateTime? updated_at { get; set; }

            public Boolean IsLoaded
            {
                get { return _id.HasValue; }
            }

            #region Fields

            private Int32? _id = null;

            #endregion
        }

        #endregion

        #region Constants

        private static class UrlTemplates
        {
            public const String Authorization = "https://github.com/login/oauth/authorize?client_id={0}&redirect_uri={1}&state={2}&scope={3}";
            public const String AccessToken = "https://github.com/login/oauth/access_token";

            public const String ApiUserEmails = "https://api.github.com/user/emails?access_token={0}";
            public const String ApiUser = "https://api.github.com/user?access_token={0}";
        }

        private const String StateSessionKey = "GitTime.Web.Infrastructure.Helpers.GitHubHelper.State";
        private const String CurrentAccessTokenItemsKey = "GitTime.Web.Infrastructure.Helpers.GitHubHelper.CurrentAccess";

        private const Int32 WebRequestTimeout = 10; // seconds

        #endregion

        #region Properties

        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public static Boolean IsConfigured
        {
            get { return !String.IsNullOrEmpty(AppSettings.GitHub.ClientID) && !String.IsNullOrEmpty(AppSettings.GitHub.SecretKey); }
        }

        private static String CurrentState
        {
            get { return (String)Session[StateSessionKey]; }
            set { Session[StateSessionKey] = value; }
        }

        private static AccessToken CurrentAccessToken
        {
            get { return (AccessToken)HttpContext.Current.Items[CurrentAccessTokenItemsKey]; }
            set { HttpContext.Current.Items[CurrentAccessTokenItemsKey] = value; }
        }

        #endregion

        #region Fields

        private static readonly JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

        #endregion

        #region Public methods

        public static Boolean IsStateValid(String state)
        {
            return !String.IsNullOrEmpty(state) && state == CurrentState;
        }

        public static String GetAuthorizationUrl(String callbackUrl, params String[] scope)
        {
            var state = CurrentState = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var strScope = scope == null || scope.Length == 0 ? null : TextHelper.ConvertListToCsvText(scope, false);

            return String.Format(UrlTemplates.Authorization, AppSettings.GitHub.ClientID, callbackUrl, state, strScope);
        }

        public async static Task<AccessToken> RequestAccessTokenAsync(String code, String state)
        {
            if (IsConfigured && !String.IsNullOrEmpty(code) && IsStateValid(state))
            {
                var response = await GetResponseAsync(
                    UrlTemplates.AccessToken,
                    new Dictionary<String, String> { 
                        { "client_id", AppSettings.GitHub.ClientID },
                        { "client_secret", AppSettings.GitHub.SecretKey },
                        { "code", code },
                        { "state", state }
                    });

                if (!String.IsNullOrEmpty(response))
                {
                    var accessToken = jsonSerializer.Deserialize<JsonAccessToken>(response);

                    if (accessToken.error != null)
                        throw GitTimeException.Create("{0}\r\nDetails: {1}", accessToken.error_description, accessToken.error_uri);

                    if (accessToken.access_token != null)
                        return new AccessToken
                        {
                            Application = "GitHub",
                            Key = accessToken.access_token,
                            UtcCreated = DateTime.UtcNow
                        };
                }
            }

            return null;
        }

        public async static Task<IEnumerable<GitHubUserEmailInfo>> RequestUserEmailsAsync(AccessToken accessToken)
        {
            var url = String.Format(UrlTemplates.ApiUserEmails, accessToken.Key);
            var response = await GetResponseAsync(url, null);
            var userEmails = jsonSerializer.Deserialize<List<JsonUserEmail>>(response);

            return userEmails != null ? userEmails.ConvertAll(r => new GitHubUserEmailInfo { Email = r.email, IsPrimary = r.primary, IsVerified = r.verified }) : null;
        }

        public async static Task<GitHubUserInfo> RequestUserInfoAsync(AccessToken accessToken)
        {
            var url = String.Format(UrlTemplates.ApiUser, accessToken.Key);
            var response = await GetResponseAsync(url, null);
            var user = jsonSerializer.Deserialize<JsonUser>(response);

            return user == null || !user.IsLoaded
                ? null
                : new GitHubUserInfo
                {
                    ID = user.id,
                    LoginName = user.login,
                    AvatarUrl = user.avatar_url,
                    GravatarId = user.gravatar_id,
                    ApiUrl = user.url,
                    HtmlUrl = user.html_url,
                    ApiFollowersUrl = user.followers_url,
                    ApiFollowingUrl = user.following_url,
                    ApiGistsUrl = user.gists_url,
                    ApiStarredUrl = user.starred_url,
                    ApiSubscriptionsUrl = user.subscriptions_url,
                    ApiOrganizationsUrl = user.organizations_url,
                    ApiReposUrl = user.repos_url,
                    ApiEventsUrl = user.events_url,
                    ApiReceivedEventsUrl = user.received_events_url,
                    AccountType = user.type,
                    IsSiteAdmin = user.site_admin,
                    Name = user.name,
                    Company = user.company,
                    BlogUrl = user.blog,
                    Location = user.location,
                    Email = user.email,
                    IsHireable = user.hireable,
                    Biography = user.bio,
                    PublicReposCount = user.public_repos,
                    PublicGistsCount = user.public_gists,
                    FollowersCount = user.followers,
                    FollowingCount = user.following,
                    CreatedOn = user.created_at,
                    UpdatedOn = user.updated_at,
                };
        }

        #endregion

        #region Helpers

        private async static Task<String> GetResponseAsync(String url, Dictionary<String, String> parameters)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage { RequestUri = new Uri(url), Method = HttpMethod.Post, };

                if (parameters == null)
                    request.Method = HttpMethod.Get;
                else
                    request.Content = new FormUrlEncodedContent(parameters);

                request.Headers.Add("User-Agent", "GitTime");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return await response.Content.ReadAsStringAsync();

                    var responseResult = await response.Content.ReadAsStringAsync();

                    var message = jsonSerializer.Deserialize<JsonMessage>(responseResult);

                    throw GitTimeException.Create("{0}\r\nDetails: {1}", message.message, message.documentation_url);
                }
            }
        }

        #endregion
    }
}