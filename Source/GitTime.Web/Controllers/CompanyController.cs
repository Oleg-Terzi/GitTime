using System.Data.Entity;
using System.Linq;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Company;

namespace GitTime.Web.Controllers
{
    public class CompanyController : BaseFinderController<FinderModel, ContactFilter>
    {
        #region Properties

        protected override string SingleEntityName
        {
            get { return "Company"; }
        }

        protected override string MultiEntityName
        {
            get { return "Companies"; }
        }

        #endregion

        #region Initialization

        protected override ContactFilter GetInitFilter()
        {
            return new ContactFilter();
        }

        protected override ContactFilter GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? GetInitFilter()
                : new ContactFilter
                    {
                        Name = model.SearchCriteria.Name
                    };
        }

        protected override BaseSearchResultsModel GetSearchResults(FinderModel model)
        {
            return model.SearchResults;
        }

        protected override void InitModel(FinderModel model, BaseSearchResultsModel searchResults, ContactFilter filter)
        {
            model.SearchResults = searchResults;

            model.SearchCriteria = new SearchCriteriaModel
            {
                Name = filter.Name
            };
        }

        protected override void InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel();
        }

        protected override void InitEdit(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = db.Companies
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditorModel
                    {
                        ID = p.ID,
                        Name = p.Name
                    }).First();
            }
        }

        #endregion


        #region Database methods

        protected override int Count(ContactFilter filter)
        {
            filter.Subtype = Constants.ContactTypes.Company;

            using (var db = new GitTimeContext())
            {
                return db.Contacts.Count(filter);
            }
        }

        protected override object Select(int startRow, int endRow, ContactFilter filter)
        {
            filter.Subtype = Constants.ContactTypes.Company;

            using (var db = new GitTimeContext())
                return db.Contacts.SelectFinderRows(startRow, endRow, filter, "Name");
        }

        protected override SaveResult SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Company row = editorModel.ID.HasValue
                    ? db.Companies.Find(editorModel.ID.Value)
                    : new Company();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This company record doesn't exist anymore. Please create new one.";
                    return SaveResult.NotSaved;
                }

                row.Name = editorModel.Name;

                if (editorModel.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Companies.Add(row);

                db.SaveChanges();
            }

            return editorModel.ID.HasValue ? SaveResult.Edited : SaveResult.Added;
        }

        protected override void DeleteData(FinderModel model)
        {
            using (GitTimeContext db = new GitTimeContext())
            {
                db.Contacts.Delete(model.Key.Value);
            }
        }

        #endregion
    }
}