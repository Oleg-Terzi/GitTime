using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Company;

namespace GitTime.Web.Controllers
{
    public class CompanyController : BaseFinderController<FinderModel, ContactFilter>
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
            get { return "Company"; }
        }

        protected override String MultiEntityName
        {
            get { return "Companies"; }
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
                        Name = model.SearchCriteria.Name
                    };
        }

        protected override BaseSearchResultsModel GetSearchResults(FinderModel model)
        {
            return model.SearchResults;
        }

        protected override async Task InitModel(FinderModel model, BaseSearchResultsModel searchResults, ContactFilter filter)
        {
            model.SearchResults = searchResults;

            model.SearchCriteria = new SearchCriteriaModel
            {
                Name = filter.Name
            };
        }

        protected override async Task InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel();
        }

        protected override async Task InitEdit(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = await db.Companies
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditorModel
                    {
                        ID = p.ID,
                        Name = p.Name
                    }).FirstAsync();
            }
        }

        #endregion


        #region Database methods

        protected override async Task<Int32> Count(ContactFilter filter)
        {
            filter.Subtype = Constants.ContactType.Company;

            using (var db = new GitTimeContext())
            {
                return await db.Contacts.CountAsync(filter);
            }
        }

        protected override async Task<Object> Select(Int32 startRow, Int32 endRow, ContactFilter filter)
        {
            filter.Subtype = Constants.ContactType.Company;

            using (var db = new GitTimeContext())
                return await db.Contacts.SelectFinderRowsAsync(startRow, endRow, filter, "Name");
        }

        protected override async Task<SaveResult> SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Company row = editorModel.ID.HasValue
                    ? await db.Companies.FindAsync(editorModel.ID.Value)
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