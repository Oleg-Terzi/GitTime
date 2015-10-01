using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using GitTime.Web;
using GitTime.Web.Infrastructure.Web;
using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Timecard;

namespace GitTime.Web.Controllers
{
    public class TimecardController : BaseFinderController<FinderModel, TimecardFilter>
    {
        #region Security

        protected override Boolean CanView(IPrincipal principal)
        {
            return principal.IsInRole(Constants.RoleType.Administrator) || principal.IsInRole(Constants.RoleType.Developer);
        }

        #endregion

        #region Properties

        protected override String SingleEntityName
        {
            get { return "Timecard"; }
        }

        protected override String MultiEntityName
        {
            get { return "Timecards"; }
        }

        #endregion

        #region Initialization

        protected override TimecardFilter GetInitFilter()
        {
            return new TimecardFilter();
        }

        protected override TimecardFilter GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? GetInitFilter()
                : new TimecardFilter
                    {
                        ProjectID = model.SearchCriteria.ProjectID,
                        PersonContactID = model.SearchCriteria.PersonContactID,
                        EntryDateFrom = model.SearchCriteria.EntryDateFrom,
                        EntryDateThru = model.SearchCriteria.EntryDateThru
                    };
        }

        protected override BaseSearchResultsModel GetSearchResults(FinderModel model)
        {
            return model.SearchResults;
        }

        protected override void InitModel(FinderModel model, BaseSearchResultsModel searchResults, TimecardFilter filter)
        {
            model.SearchResults = searchResults;

            model.SearchCriteria = new SearchCriteriaModel
            {
                ProjectID = filter.ProjectID,
                PersonContactID = filter.PersonContactID,
                EntryDateFrom = filter.EntryDateFrom,
                EntryDateThru = filter.EntryDateThru
            };
        }

        protected override async Task InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel { EntryDate = DateTime.Now };

            using (var db = new GitTimeContext())
            {
                model.Edit.PersonContactID = await db.Persons
                    .Where(p => p.Email == User.Identity.Name)
                    .Select(p => p.ID).FirstAsync();
            }
        }

        protected override async Task InitEdit(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = await db.Timecards
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditorModel
                    {
                        ID = p.ID,
                        EntryDate = p.EntryDate,
                        PersonContactID = p.PersonContactID,
                        ProjectID = p.ProjectID,
                        IssueNumber = p.IssueNumber,
                        IssueDescription = p.IssueDescription,
                        Hours = p.Hours
                    }).FirstAsync();
            }
        }

        #endregion

        #region Database methods

        protected override async Task<Int32> Count(TimecardFilter filter)
        {
            using (var db = new GitTimeContext())
            {
                ViewBag.SumHours = await db.Timecards.SumHoursAsync(filter);

                return await db.Timecards.CountAsync(filter);
            }
        }

        protected override async Task<Object> Select(Int32 startRow, Int32 endRow, TimecardFilter filter)
        {
            using (var db = new GitTimeContext())
                return await db.Timecards.SelectFinderRowsAsync(startRow, endRow, filter, null);
        }

        protected override async Task<SaveResult> SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Timecard row = editorModel.ID.HasValue
                    ? await db.Timecards.FindAsync(editorModel.ID.Value)
                    : new Timecard();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This timecard doesn't exist anymore. Please create new one.";
                    return SaveResult.NotSaved;
                }

                row.EntryDate = editorModel.EntryDate;
                row.PersonContactID = editorModel.PersonContactID.Value;
                row.IssueNumber = editorModel.IssueNumber;
                row.Hours = editorModel.Hours.Value;
                row.ProjectID = editorModel.ProjectID.Value;
                row.IssueDescription = editorModel.IssueDescription;

                if (editorModel.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Timecards.Add(row);

                await db.SaveChangesAsync();
            }

            return editorModel.ID.HasValue ? SaveResult.Edited : SaveResult.Added;
        }

        protected override async Task DeleteData(FinderModel model)
        {
            using (var db = new GitTimeContext())
                await db.Timecards.DeleteAsync(model.Key.Value);
        }

        #endregion
    }
}