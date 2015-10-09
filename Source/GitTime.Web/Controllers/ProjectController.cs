using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Project;

namespace GitTime.Web.Controllers
{
    public class ProjectController : BaseFinderController<FinderModel, ProjectFilter>
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
            get { return "Project"; }
        }

        protected override String MultiEntityName
        {
            get { return "Projects"; }
        }

        #endregion

        #region Initialization

        protected override async Task<ProjectFilter> GetInitFilter()
        {
            return new ProjectFilter();
        }

        protected override async Task<ProjectFilter> GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? await GetInitFilter()
                : new ProjectFilter
                    {
                        CompanyContactID = model.SearchCriteria.CompanyContactID
                    };
        }

        protected override BaseSearchResultsModel GetSearchResults(FinderModel model)
        {
            return model.SearchResults;
        }

        protected override void InitModel(FinderModel model, BaseSearchResultsModel searchResults, ProjectFilter filter)
        {
            model.SearchResults = searchResults;

            model.SearchCriteria = new SearchCriteriaModel
            {
                CompanyContactID = filter.CompanyContactID
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
                model.Edit = await db.Projects
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditorModel
                    {
                        ID = p.ID,
                        CompanyContactID = p.CompanyContactID,
                        Name = p.Name,
                        Description = p.Description,
                        Repository = p.Repository
                    }).FirstAsync();
            }
        }

        #endregion
        
        #region Database methods

        protected override async Task<Int32> Count(ProjectFilter filter)
        {
            using (var db = new GitTimeContext())
            {
                return await db.Projects.CountAsync(filter);
            }
        }

        protected override async Task<Object> Select(Int32 startRow, Int32 endRow, ProjectFilter filter)
        {
            using (var db = new GitTimeContext())
                return await db.Projects.SelectFinderRowsAsync(startRow, endRow, filter, null);
        }

        protected override async Task<SaveResult> SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                var row = editorModel.ID.HasValue
                    ? await db.Projects.FindAsync(editorModel.ID.Value)
                    : new Project();

                if (row == null)
                {
                    ViewBag.ErrorMessage = "This project doesn't exist anymore. Please create new one.";
                    return SaveResult.NotSaved;
                }

                row.CompanyContactID = editorModel.CompanyContactID.Value;
                row.Name = editorModel.Name;
                row.Description = editorModel.Description;
                row.Repository = editorModel.Repository;

                if (editorModel.ID.HasValue)
                    db.Entry(row).State = EntityState.Modified;
                else
                    db.Projects.Add(row);

                await db.SaveChangesAsync();
            }

            return editorModel.ID.HasValue ? SaveResult.Edited : SaveResult.Added;
        }

        protected override async Task DeleteData(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                await db.Projects.DeleteAsync(model.Key.Value);
            }
        }

        #endregion
    }
}