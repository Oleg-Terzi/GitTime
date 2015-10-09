using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            #region Public methods

            public static GitHubUserInfo CreateInfo(JsonUser user, GitHubContext context)
            {
                GitHubUserInfo result = null;

                if (user != null && user.IsLoaded)
                {
                    result = new GitHubUserInfo
                    {
                        ID = user.id,
                        LoginName = user.login,
                        AvatarUrl = user.avatar_url,
                        GravatarId = user.gravatar_id,
                        ProfileUrl = user.html_url,
                        AccountType = (GitHubUserType)Enum.Parse(typeof(GitHubUserType), user.type),
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

                    if (context != null)
                        result = context.AddUser(result);
                }

                return result;
            }

            #endregion
        }

        private class JsonLabel
        {
            public String url { get; set; }
            public String name { get; set; }
            public String color { get; set; }

            #region Public methods

            public static GitHubLabelInfo CreateInfo(JsonLabel label, GitHubContext context)
            {
                GitHubLabelInfo result = null;

                if (label != null)
                {
                    result = new GitHubLabelInfo { Name = label.name, Color = label.color, };
                    if (context != null)
                        result = context.AddLabel(result);
                }

                return result;
            }

            public static GitHubLabelInfo[] CreateInfoArray(IEnumerable<JsonLabel> labels, GitHubContext context)
            {
                var result = new List<GitHubLabelInfo>();

                if (labels != null)
                {
                    foreach (JsonLabel label in labels)
                        result.Add(JsonLabel.CreateInfo(label, context));
                }

                return result.ToArray();
            }

            #endregion
        }

        private class JsonRepository
        {
            public Int32 id { get; set; }
            public String name { get; set; }
            public String full_name { get; set; }
            public Boolean @private { get; set; }
            public String html_url { get; set; }
            public String description { get; set; }
            public Boolean fork { get; set; }
            public String url { get; set; }
            public String forks_url { get; set; }
            public String keys_url { get; set; }
            public String collaborators_url { get; set; }
            public String teams_url { get; set; }
            public String hooks_url { get; set; }
            public String issue_events_url { get; set; }
            public String events_url { get; set; }
            public String assignees_url { get; set; }
            public String branches_url { get; set; }
            public String tags_url { get; set; }
            public String blobs_url { get; set; }
            public String git_tags_url { get; set; }
            public String git_refs_url { get; set; }
            public String trees_url { get; set; }
            public String statuses_url { get; set; }
            public String languages_url { get; set; }
            public String stargazers_url { get; set; }
            public String contributors_url { get; set; }
            public String subscribers_url { get; set; }
            public String subscription_url { get; set; }
            public String commits_url { get; set; }
            public String git_commits_url { get; set; }
            public String comments_url { get; set; }
            public String issue_comment_url { get; set; }
            public String contents_url { get; set; }
            public String compare_url { get; set; }
            public String merges_url { get; set; }
            public String archive_url { get; set; }
            public String downloads_url { get; set; }
            public String issues_url { get; set; }
            public String pulls_url { get; set; }
            public String milestones_url { get; set; }
            public String notifications_url { get; set; }
            public String labels_url { get; set; }
            public String releases_url { get; set; }
            public DateTime created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public DateTime? pushed_at { get; set; }
            public String git_url { get; set; }
            public String ssh_url { get; set; }
            public String clone_url { get; set; }
            public String svn_url { get; set; }
            public String homepage { get; set; }
            public Int32 size { get; set; }
            public Int32 stargazers_count { get; set; }
            public Int32 watchers_count { get; set; }
            public String language { get; set; }
            public Boolean has_issues { get; set; }
            public Boolean has_downloads { get; set; }
            public Boolean has_wiki { get; set; }
            public Boolean has_pages { get; set; }
            public Int32 forks_count { get; set; }
            public String mirror_url { get; set; }
            public Int32 open_issues_count { get; set; }
            public Int32 forks { get; set; }
            public Int32 open_issues { get; set; }
            public Int32 watchers { get; set; }
            public String default_branch { get; set; }

            public JsonUser owner { get; set; }

            #region Public methods

            public static GitHubRepositoryInfo CreateInfo(JsonRepository repository, GitHubContext context)
            {
                GitHubRepositoryInfo result = null;

                if (repository != null)
                {
                    result = new GitHubRepositoryInfo { 
                        ID = repository.id,
                        Name = repository.name,
                        FullName = repository.full_name,
                        IsPrivate = repository.@private,
                        Url = repository.url,
                        Description = repository.description,
                        IsFork = repository.fork,
                        CreatedOn = repository.created_at,
                        UpdatedOn = repository.updated_at,
                        PushedOn = repository.pushed_at,
                        HomepageUrl = repository.homepage,
                        Size = repository.size,
                        StargazersCount = repository.stargazers_count,
                        WatchersCount = repository.watchers,
                        Language = repository.language,
                        HasIssues = repository.has_issues,
                        HasDownloads = repository.has_downloads,
                        HasWiki = repository.has_wiki,
                        HasPages = repository.has_pages,
                        ForksCount = repository.forks,
                        MirrorUrl = repository.mirror_url,
                        OpenIssuesCount = repository.open_issues,
                        DefaultBranchName = repository.default_branch,
                        Owner = JsonUser.CreateInfo(repository.owner, context),
                    };

                    if (context != null)
                        result = context.AddRepository(result);
                }

                return result;
            }
            
            #endregion
        }

        private class JsonMilestone
        {
            public String url { get; set; }
            public String html_url { get; set; }
            public String labels_url { get; set; }
            public Int32 id { get; set; }
            public Int32 number { get; set; }
            public String state { get; set; }
            public String title { get; set; }
            public String description { get; set; }
            public Int32 open_issues { get; set; }
            public Int32 closed_issues { get; set; }
            public DateTime created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public DateTime? closed_at { get; set; }
            public DateTime? due_on { get; set; }

            public JsonUser creator { get; set; }
        }

        private class JsonIssue
        {
            public String url { get; set; }
            public String labels_url { get; set; }
            public String comments_url { get; set; }
            public String events_url { get; set; }
            public String html_url { get; set; }
            public Int32 id { get; set; }
            public Int32 number { get; set; }
            public String title { get; set; }
            public String state { get; set; }
            public Boolean locked { get; set; }
            public Int32 comments { get; set; }
            public DateTime created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public DateTime? closed_at { get; set; }
            public String body { get; set; }

            public JsonMilestone milestone { get; set; }
            public JsonUser user { get; set; }
            public JsonLabel[] labels { get; set; }
            public JsonUser assignee { get; set; }
            public JsonRepository repository { get; set; }

            #region Public methods

            public static GitHubIssueInfo CreateInfo(JsonIssue issue, GitHubContext context)
            {
                var result = new GitHubIssueInfo();

                result.ID = issue.id;
                result.IssueUrl = issue.url;
                result.Number = issue.number;
                result.Title = issue.title;
                result.State = (GitHubIssueStateType)Enum.Parse(typeof(GitHubIssueStateType), issue.state, true);
                result.IsLocked = issue.locked;
                result.CommentsCount = issue.comments;
                result.CreatedOn = issue.created_at;
                result.UpdatedOn = issue.updated_at;
                result.ClosedOn = issue.closed_at;
                result.BodyText = issue.body;
                result.User = JsonUser.CreateInfo(issue.user, context);
                result.Assignee = JsonUser.CreateInfo(issue.assignee, context);
                result.Labels = JsonLabel.CreateInfoArray(issue.labels, context);
                result.Repository = JsonRepository.CreateInfo(issue.repository, context);

                if (context != null)
                    result = context.AddIssue(result);

                return result;
            }

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
            public const String ApiIssues = "https://api.github.com/issues?filter=all&state=all&access_token={0}";
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

            return JsonUser.CreateInfo(user, null);
        }

        public async static Task<GitHubContext> RequestIssuesAsync(AccessToken accessToken)
        {
            var url = String.Format(UrlTemplates.ApiIssues, accessToken.Key);
            var response = await GetResponseAsync(url, null);

            var issues = jsonSerializer.Deserialize<List<JsonIssue>>(response);

            var context = new GitHubContext();
            foreach (JsonIssue issue in issues)
                JsonIssue.CreateInfo(issue, context);

            return context;
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