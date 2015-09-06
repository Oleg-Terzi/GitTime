using System.Data.Entity;
using System.Linq;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Person;

namespace GitTime.Web.Controllers
{
    public class PersonController : BaseFinderController<FinderModel, ContactFilter>
    {
        #region Properties

        protected override string SingleEntityName
        {
            get { return "Person"; }
        }

        protected override string MultiEntityName
        {
            get { return "Persons"; }
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
                        PersonName = model.SearchCriteria.PersonName
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
                PersonName = filter.PersonName
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
                model.Edit = db.Persons
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditorModel
                    {
                        ID = p.ID,
                        Email = p.Email,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Password = p.Password
                    }).First();
            }
        }

        #endregion


        #region Database methods

        protected override int Count(ContactFilter filter)
        {
            filter.Subtype = Constants.ContactTypes.Person;

            using (var db = new GitTimeContext())
            {
                return db.Contacts.Count(filter);
            }
        }

        protected override object Select(int startRow, int endRow, ContactFilter filter)
        {
            filter.Subtype = Constants.ContactTypes.Person;

            using (var db = new GitTimeContext())
                return db.Contacts.SelectFinderRows(startRow, endRow, filter, "FirstName, LastName");
        }

        protected override SaveResult SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Person row = editorModel.ID.HasValue
                    ? db.Persons.Find(editorModel.ID.Value)
                    : new Person();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This person record doesn't exist anymore. Please create new one.";
                    return SaveResult.NotSaved;
                }

                row.Email = editorModel.Email;
                row.FirstName = editorModel.FirstName;
                row.LastName = editorModel.LastName;
                row.Password = editorModel.Password;

                if (editorModel.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Persons.Add(row);

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