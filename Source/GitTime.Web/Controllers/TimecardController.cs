using System;
using System.Data.Entity;
using System.Linq;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Timecard;

namespace GitTime.Web.Controllers
{
    public class TimecardController : BaseFinderController<FinderModel, TimecardFilter>
    {
        #region Properties

        protected override string SingleEntityName
        {
            get { return "Timecard"; }
        }

        protected override string MultiEntityName
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

        protected override void InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel() { EntryDate = DateTime.Now };

            using (var db = new GitTimeContext())
            {
                model.Edit.PersonContactID = db.Persons
                    .Where(p => p.Email == User.Identity.Name)
                    .Select(p => p.ID).First();
            }
        }

        protected override void InitEdit(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = db.Timecards
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

        protected override SaveResult SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Timecard row = editorModel.ID.HasValue
                    ? db.Timecards.Find(editorModel.ID.Value)
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

                db.SaveChanges();
            }

            return editorModel.ID.HasValue ? SaveResult.Edited: SaveResult.Added;
        }

        protected override void DeleteData(FinderModel model)
        {
            using (GitTimeContext db = new GitTimeContext())
            {
                db.Timecards.Delete(model.Key.Value);
            }
        }

        #endregion
    }
}