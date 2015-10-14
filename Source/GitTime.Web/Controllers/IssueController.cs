using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using GitTime.Web.Infrastructure.GitHub;
using GitTime.Web.Infrastructure.GitHub.Data;
using GitTime.Web.Infrastructure.Web;
using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Issue;

namespace GitTime.Web.Controllers
{
    public class IssueController : BaseFinderController<FinderModel, GitHubIssueFilter>
    {
        #region Constants

        private const String IssuesSessionStateKey = "GitTime.Web.Controllers.IssueController.Issues";

        private const Int32 IssuesRefreshPeriod = 5;

        #endregion

        #region Security

        protected override Boolean CanView(IPrincipal principal)
        {
            var user = GitHubUser.GetCurrent();

            return user.IsAuthenticated && (principal.IsInRole(Constants.RoleType.Administrator) || principal.IsInRole(Constants.RoleType.Developer));
        }

        #endregion

        #region Properties

        protected override String SingleEntityName
        {
            get { return "Issue"; }
        }

        protected override String MultiEntityName
        {
            get { return "Issues"; }
        }

        #endregion

        #region Fields

        private ICollection<GitHubIssueInfo> _issues = null;

        #endregion

        #region Initialization

        protected override async Task<GitHubIssueFilter> GetInitFilter()
        {
            return new GitHubIssueFilter { State = GitHubIssueStateType.Open };
        }

        protected override async Task<GitHubIssueFilter> GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? await GetInitFilter()
                : new GitHubIssueFilter
                {
                    RepositoryName = model.SearchCriteria.RepositoryName,
                    State = !String.IsNullOrEmpty(model.SearchCriteria.StatusName)
                        ? (GitHubIssueStateType?)Enum.Parse(typeof(GitHubIssueStateType), model.SearchCriteria.StatusName)
                        : null
                };
        }

        protected override BaseSearchResultsModel GetSearchResults(FinderModel model)
        {
            return model.SearchResults;
        }

        protected override async Task InitModel(FinderModel model, BaseSearchResultsModel searchResults, GitHubIssueFilter filter)
        {
            model.SearchResults = searchResults;
            model.SearchCriteria = new SearchCriteriaModel
            {
                RepositoryName = filter.RepositoryName,
                StatusName = filter.State.HasValue ? Enum.GetName(typeof(GitHubIssueStateType), filter.State.Value) : null,
            };
        }

        protected override async Task InitCreate(FinderModel model)
        {
            throw new NotImplementedException();
        }

        protected override async Task InitEdit(FinderModel model)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Database methods

        protected override async Task OnBeforeLoadData(GitHubIssueFilter filter)
        {
            using (var db = new GitTimeContext())
            {
                if (filter.AvailableRepositories == null)
                    filter.AvailableRepositories = await db.Projects.Select(p => p.Repository).ToArrayAsync();

                ViewBag.IssueHours = await db.Timecards
                    .Where(t => t.IssueNumber.HasValue)
                    .GroupBy(t => new { t.Project.Repository, t.IssueNumber })
                    .Select(g =>
                        new ViewerModel.IssueHoursData
                        {
                            Number = g.FirstOrDefault().IssueNumber.Value,
                            RepositoryName = g.FirstOrDefault().Project.Repository,
                            Hours = g.Sum(t => t.Hours)
                        })
                    .ToListAsync()
                    ;
            }

            if (!filter.UserID.HasValue)
            {
                var user = await GitHubUser.GetCurrentAsync();
                filter.UserID = user.ID;
            }

            var context = await GetContext(Session);

            _issues = context.Issues
                .Select(i => i.Value)
                .Where(i => IsMatchFilter(filter, i))
                .OrderBy(i => i.Number)
                .ToList();
        }

        private static Boolean IsMatchFilter(GitHubIssueFilter filter, GitHubIssueInfo info)
        {
            return filter == null || (
                (filter.AvailableRepositories != null && filter.AvailableRepositories.Length > 0 && filter.AvailableRepositories.Contains(info.Repository.FullName))
                && (String.IsNullOrEmpty(filter.RepositoryName) || info.Repository.FullName == filter.RepositoryName)
                && (filter.State == null || info.State == filter.State)
                && (
                    !filter.UserID.HasValue
                    || filter.UserID.HasValue && (
                        info.Assignee != null && info.Assignee.ID == filter.UserID.Value
                        || info.User != null && info.User.ID == filter.UserID.Value
                    )
                   )
            );
        }

        protected override async Task<Int32> Count(GitHubIssueFilter filter)
        {
            return _issues.Count;
        }

        protected override async Task<Object> Select(Int32 startRow, Int32 endRow, GitHubIssueFilter filter)
        {
            var skipCount = startRow - 1;
            if (skipCount < 0)
                skipCount = 0;

            var takeCount = endRow - startRow + 1;
            if (takeCount >= 0)
            {
                var maxTakeCount = _issues.Count - skipCount;
                if (maxTakeCount < takeCount)
                    takeCount = maxTakeCount;
            }
            else
            {
                takeCount = 0;
            }

            return _issues.Skip(skipCount).Take(takeCount).ToList();
        }

        protected override async Task<SaveResult> SaveData(FinderModel model)
        {
            throw new NotImplementedException();
        }

        protected override async Task DeleteData(FinderModel model)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Views

        [HttpPost]
        public async Task<ActionResult> ViewIssue(Int32 id, String tab)
        {
            var user = await GitHubUser.GetCurrentAsync();
            var context = await GetContext(Session);
            var issue = await GitHubHelper.RequestIssueCommentsAsync(id, user.AccessToken, context);

            var model = new ViewerModel
            {
                View = new ViewerModel.ViewData
                {
                    ID = id,
                    Number = issue.Number,
                    Title = issue.Title,
                    BodyText = issue.BodyText,
                    ActiveTab = tab,
                    Comments = issue.Comments,
                },
                Timecard = new ViewerModel.TimecardData
                {
                    EntryDate = DateTime.UtcNow,
                    Hours = 0,
                }
            };

            model.View.Timecards = await GetIssueTimecards(issue);

            return PartialView("View", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTimecard(ViewerModel model)
        {
            var isError = false;

            var gitTimeUser = GitTimeUser.Current;
            var context = await GetContext(Session);
            var issue = context.Issues[model.View.ID];

            if (model.Timecard.Hours > 0 && model.Timecard.EntryDate > DateTime.MinValue)
            {
                using (var db = new GitTimeContext())
                {
                    var project = await db.Projects
                        .Where(p => p.Repository == issue.Repository.FullName)
                        .FirstAsync()
                        ;
                    var timecard = await db.Timecards
                        .Where(t =>
                            t.IssueNumber == issue.Number
                            && t.PersonContactID == gitTimeUser.ID
                            && t.ProjectID == project.ID
                            && DbFunctions.TruncateTime(t.EntryDate) == DbFunctions.TruncateTime(model.Timecard.EntryDate)
                         )
                        .FirstOrDefaultAsync()
                        ;

                    if (timecard == null)
                    {
                        timecard = new Timecard
                        {
                            EntryDate = model.Timecard.EntryDate,
                            PersonContactID = gitTimeUser.ID,
                            IssueNumber = issue.Number,
                            Hours = model.Timecard.Hours,
                            ProjectID = project.ID,
                            IssueDescription = String.Format("Issue #{0:0000}: {1}", issue.Number, issue.Title),
                        };

                        db.Timecards.Add(timecard);
                    }
                    else
                    {
                        timecard.Hours += model.Timecard.Hours;

                        db.Entry(timecard).State = EntityState.Modified;
                    }

                    await db.SaveChangesAsync();
                }
            }
            else
                isError = true;

            var partialModel = new ViewerModel 
            {
                View = new ViewerModel.ViewData { ID = model.View.ID }
            };

            partialModel.View.Timecards = await GetIssueTimecards(issue);
            partialModel.Timecard = isError
                ? new ViewerModel.TimecardData { EntryDate = model.Timecard.EntryDate, Hours = model.Timecard.Hours }
                : new ViewerModel.TimecardData { EntryDate = DateTime.UtcNow, Hours = 0 };

            return PartialView("IssueTimecards", partialModel);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTimecard(ViewerModel model)
        {
            using (var db = new GitTimeContext())
            {
                var timecard = await db.Timecards.Where(t => t.ID == model.Timecard.ID).FirstAsync();
                if (timecard != null && timecard.PersonContactID == GitTimeUser.Current.ID)
                    await db.Timecards.DeleteAsync(timecard.ID);
            }

            var context = await GetContext(Session);
            var issue = context.Issues[model.View.ID];
            var partialModel = new ViewerModel
            {
                View = new ViewerModel.ViewData { ID = model.View.ID }
            };

            partialModel.View.Timecards = await GetIssueTimecards(issue);
            partialModel.Timecard = new ViewerModel.TimecardData { EntryDate = model.Timecard.EntryDate, Hours = model.Timecard.Hours };

            return PartialView("IssueTimecards", partialModel);
        }

        #endregion

        #region Helper methods

        private async Task<ViewerModel.TimecardData[]> GetIssueTimecards(GitHubIssueInfo issue)
        {
            using (var db = new GitTimeContext())
            {
                return await db.Timecards
                    .Where(t => t.IssueNumber == issue.Number && t.Project.Repository == issue.Repository.FullName)
                    .OrderBy(t => t.EntryDate)
                    .Select(t =>
                        new ViewerModel.TimecardData
                        {
                            ID = t.ID,
                            EntryDate = t.EntryDate,
                            PersonID = t.Person.ID,
                            PersonFirstName = t.Person.FirstName,
                            PersonLastName = t.Person.LastName,
                            Hours = t.Hours,
                        }
                    )
                    .ToArrayAsync()
                    ;
            }
        }

        private async static Task<GitHubContext> GetContext(HttpSessionStateBase session)
        {
            var data = (Tuple<DateTime, GitHubContext>)session[IssuesSessionStateKey];

            if (data == null || (DateTime.UtcNow - data.Item1).TotalMinutes >= IssuesRefreshPeriod)
            {
                var user = await GitHubUser.GetCurrentAsync();

                session[IssuesSessionStateKey] = data = new Tuple<DateTime, GitHubContext>(DateTime.UtcNow, await GitHubHelper.RequestIssuesAsync(user.AccessToken));
            }

            return data.Item2;
        }

        #endregion
    }
}