using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Person;

namespace GitTime.Web.Controllers
{
    public class PersonController : BaseFinderController<FinderModel, ContactFilter>
    {
        #region Security

        protected override Boolean CanView(IPrincipal principal)
        {
            return principal.IsInRole(Constants.RoleType.Administrator);
        }

        #endregion

        #region Properties

        protected override String SingleEntityName
        {
            get { return "Person"; }
        }

        protected override String MultiEntityName
        {
            get { return "Persons"; }
        }

        #endregion

        #region Initialization

        protected override async Task<ContactFilter> GetInitFilter()
        {
            return new ContactFilter();
        }

        protected override async Task<ContactFilter> GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? await GetInitFilter()
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

        protected override async Task InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel();

            using (var db = new GitTimeContext())
            {
                ViewBag.Roles = await db.Roles.ToListAsync();
            }
        }

        protected override async Task InitEdit(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                var entity = await db.Persons
                    .Where(p => p.ID == model.Key.Value)
                    .FirstAsync();

                model.Edit = new EditorModel
                {
                    ID = entity.ID,
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Password = entity.Password,
                    Roles = entity.Roles.Select(r => r.ID).ToList(),
                };

                ViewBag.Roles = await db.Roles.ToListAsync();
            }
        }

        #endregion


        #region Database methods

        protected override async Task<Int32> Count(ContactFilter filter)
        {
            filter.Subtype = Constants.ContactType.Person;

            using (var db = new GitTimeContext())
            {
                return await db.Contacts.CountAsync(filter);
            }
        }

        protected override async Task<Object> Select(Int32 startRow, Int32 endRow, ContactFilter filter)
        {
            filter.Subtype = Constants.ContactType.Person;

            using (var db = new GitTimeContext())
                return await db.Contacts.SelectFinderRowsAsync(startRow, endRow, filter, "FirstName, LastName");
        }

        protected override async Task<SaveResult> SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                var row = editorModel.ID.HasValue
                    ? await db.Persons.FindAsync(editorModel.ID.Value)
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

                row.Roles.Clear();
                foreach (var r in db.Roles.Where(r => editorModel.Roles.Contains(r.ID)))
                    row.Roles.Add(r);

                if (editorModel.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Persons.Add(row);

                await db.SaveChangesAsync();
            }

            return editorModel.ID.HasValue ? SaveResult.Edited : SaveResult.Added;
        }

        protected override async Task DeleteData(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                await db.Contacts.DeleteAsync(model.Key.Value);
            }
        }

        #endregion
    }
}