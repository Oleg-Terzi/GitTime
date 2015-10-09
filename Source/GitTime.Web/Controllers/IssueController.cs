using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

using GitTime.Web.Infrastructure.GitHub;
using GitTime.Web.Infrastructure.GitHub.Data;
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
            return principal.IsInRole(Constants.RoleType.Administrator) || principal.IsInRole(Constants.RoleType.Developer);
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

        private ICollection<GitHubIssueInfo> _data = null;

        #endregion

        #region Initialization

        protected override async Task<GitHubIssueFilter> GetInitFilter()
        {
            var user = await GitHubUser.GetCurrentAsync();

            return new GitHubIssueFilter { AssigneeID = user.ID };
        }

        protected override async Task<GitHubIssueFilter> GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? await GetInitFilter()
                : new GitHubIssueFilter { AssigneeID = model.SearchCriteria.AssigneeID };
        }

        protected override BaseSearchResultsModel GetSearchResults(FinderModel model)
        {
            return model.SearchResults;
        }

        protected override void InitModel(FinderModel model, BaseSearchResultsModel searchResults, GitHubIssueFilter filter)
        {
            model.SearchResults = searchResults;

            model.SearchCriteria = new SearchCriteriaModel
            {
                AssigneeID = filter.AssigneeID
            };
        }

        protected override async Task InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel();

            //using (var db = new GitTimeContext())
            //{
            //    ViewBag.Roles = await db.Roles.ToListAsync();
            //}
        }

        protected override async Task InitEdit(FinderModel model)
        {
            //using (var db = new GitTimeContext())
            //{
            //    var entity = await db.Persons
            //        .Where(p => p.ID == model.Key.Value)
            //        .FirstAsync();

            //    model.Edit = new EditorModel
            //    {
            //        ID = entity.ID,
            //        Email = entity.Email,
            //        FirstName = entity.FirstName,
            //        LastName = entity.LastName,
            //        Password = entity.Password,
            //        Roles = entity.Roles.Select(r => r.ID).ToList(),
            //    };

            //    ViewBag.Roles = await db.Roles.ToListAsync();
            //}
        }

        #endregion

        #region Database methods

        protected override async Task OnBeforeLoadData(GitHubIssueFilter filter)
        {
            var context = await GetContext(Session);

            _data = context.Issues.Select(i => i.Value).Where(i => IsMatchFilter(filter, i)).OrderBy(i => i.Number).ToList();
        }

        private static Boolean IsMatchFilter(GitHubIssueFilter filter, GitHubIssueInfo info)
        {
            return true;
            //return filter == null || (
            //    (!filter.AssigneeID.HasValue || (info.Assignee != null && info.Assignee.ID == filter.AssigneeID.Value))
            //);
        }

        protected override async Task<Int32> Count(GitHubIssueFilter filter)
        {
            return _data.Count;
        }

        protected override async Task<Object> Select(Int32 startRow, Int32 endRow, GitHubIssueFilter filter)
        {
            var skipCount = startRow - 1;
            if (skipCount < 0)
                skipCount = 0;

            var takeCount = endRow - startRow + 1;
            if (takeCount >= 0)
            {
                var maxTakeCount = _data.Count - skipCount;
                if (maxTakeCount < takeCount)
                    takeCount = maxTakeCount;
            }
            else
            {
                takeCount = 0;
            }

            return _data.Skip(skipCount).Take(takeCount).ToList();
        }

        protected override async Task<SaveResult> SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            //using (var db = new GitTimeContext())
            //{
            //    var row = editorModel.ID.HasValue
            //        ? await db.Persons.FindAsync(editorModel.ID.Value)
            //        : new Person();

            //    if (row == null)
            //    {
            //        ViewBag.ErrorMessage = "This person record doesn't exist anymore. Please create new one.";

            //        return SaveResult.NotSaved;
            //    }

            //    row.Email = editorModel.Email;
            //    row.FirstName = editorModel.FirstName;
            //    row.LastName = editorModel.LastName;
            //    row.Password = editorModel.Password;

            //    row.Roles.Clear();
            //    foreach (var r in db.Roles.Where(r => editorModel.Roles.Contains(r.ID)))
            //        row.Roles.Add(r);

            //    if (editorModel.ID.HasValue)
            //        db.Entry(row).State = EntityState.Modified;
            //    else
            //        db.Persons.Add(row);

            //    await db.SaveChangesAsync();
            //}

            return editorModel.ID.HasValue ? SaveResult.Edited : SaveResult.Added;
        }

        protected override async Task DeleteData(FinderModel model)
        {
            //using (var db = new GitTimeContext())
            //{
            //    await db.Contacts.DeleteAsync(model.Key.Value);
            //}
        }

        #endregion

        #region Helper methods

        private async static Task<GitHubContext> GetContext(HttpSessionStateBase session)
        {
            var data = (Tuple<DateTime, GitHubContext>)session[IssuesSessionStateKey];

            if (data == null || (DateTime.UtcNow - data.Item1).TotalMinutes >= IssuesRefreshPeriod)
            {
                var user = await GitHubUser.GetCurrentAsync();

                session[IssuesSessionStateKey] = data = new Tuple<DateTime,GitHubContext>(DateTime.UtcNow, await GitHubHelper.RequestIssuesAsync(user.AccessToken));
            }

            return data.Item2;
        }

        #endregion
    }
}