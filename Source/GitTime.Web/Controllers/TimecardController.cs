using System;
using System.Data.Entity;
using System.Linq;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Timecard;

namespace GitTime.Web.Controllers
{
    public class TimecardController : BaseFindController<FindModel, TimecardFilter>
    {
        #region Initialization
        
        protected override TimecardFilter GetInitFilter()
        {
            return new TimecardFilter();
        }

        protected override TimecardFilter GetFilterBySearchCriteria(FindModel model)
        {
            return new TimecardFilter
            {
                ProjectID = model.SearchCriteria.ProjectID,
                PersonContactID = model.SearchCriteria.PersonContactID,
                EntryDateFrom = model.SearchCriteria.EntryDateFrom,
                EntryDateThru = model.SearchCriteria.EntryDateThru
            };
        }

        protected override BaseSearchResultsModel GetSearchResults(FindModel model)
        {
            return model.SearchResults;
        }

        protected override void InitModel(FindModel model, BaseSearchResultsModel searchResults, TimecardFilter filter)
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

        protected override void InitCreate(FindModel model)
        {
            model.Edit = new EditModel() { EntryDate = DateTime.Now };

            using (var db = new GitTimeContext())
            {
                model.Edit.PersonContactID = db.Persons
                    .Where(p => p.Email == User.Identity.Name)
                    .Select(p => p.ID).First();
            }
        }

        protected override void InitEdit(FindModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = db.Timecards
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditModel
                    {
                        ID = p.ID,
                        EntryDate = p.EntryDate,
                        PersonContactID = p.PersonContactID,
                        ProjectID = p.ProjectID,
                        IssueNumber = p.IssueNumber,
                        IssueDescription = p.IssueDescription,
                        Hours = p.Hours
                    }).First();
            }
        }

        #endregion


        #region Database methods

        protected override int Count(TimecardFilter filter)
        {
            using (var db = new GitTimeContext())
            {
                ViewBag.SumHours = db.Timecards.SumHours(filter);

                return db.Timecards.Count(filter);
            }
        }

        protected override object Select(int startRow, int endRow, TimecardFilter filter)
        {
            using (var db = new GitTimeContext())
                return db.Timecards.SelectFinderRows(startRow, endRow, filter, null);
        }

        protected override SaveResult SaveData(FindModel model)
        {
            EditModel editModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Timecard row = editModel.ID.HasValue
                    ? db.Timecards.Find(editModel.ID.Value)
                    : new Timecard();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This timecard doesn't exist anymore. Please create new one.";
                    return SaveResult.NotSaved;
                }

                row.EntryDate = editModel.EntryDate;
                row.PersonContactID = editModel.PersonContactID.Value;
                row.IssueNumber = editModel.IssueNumber;
                row.Hours = editModel.Hours.Value;
                row.ProjectID = editModel.ProjectID.Value;
                row.IssueDescription = editModel.IssueDescription;

                if (editModel.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Timecards.Add(row);

                db.SaveChanges();
            }

            return editModel.ID.HasValue ? SaveResult.Edited: SaveResult.Edited;
        }

        protected override void DeleteData(FindModel model)
        {
            using (GitTimeContext db = new GitTimeContext())
            {
                db.Timecards.Delete(model.Key.Value);
            }
        }

        #endregion
    }
}