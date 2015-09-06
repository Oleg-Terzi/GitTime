using System.Data.Entity;
using System.Linq;

using GitTime.Web.Models;
using GitTime.Web.Models.Database;
using GitTime.Web.Models.View;
using GitTime.Web.Models.View.Project;

namespace GitTime.Web.Controllers
{
    public class ProjectController : BaseFinderController<FinderModel, ProjectFilter>
    {
        #region Properties

        protected override string SingleEntityName
        {
            get { return "Project"; }
        }

        protected override string MultiEntityName
        {
            get { return "Projects"; }
        }

        #endregion

        #region Initialization

        protected override ProjectFilter GetInitFilter()
        {
            return new ProjectFilter();
        }

        protected override ProjectFilter GetFilterBySearchCriteria(FinderModel model)
        {
            return model.SearchCriteria.Clear
                ? GetInitFilter()
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

        protected override void InitCreate(FinderModel model)
        {
            model.Edit = new EditorModel();
        }

        protected override void InitEdit(FinderModel model)
        {
            using (var db = new GitTimeContext())
            {
                model.Edit = db.Projects
                    .Where(p => p.ID == model.Key.Value)
                    .Select(p => new EditorModel
                    {
                        ID = p.ID,
                        CompanyContactID = p.CompanyContactID,
                        Name = p.Name,
                        Description = p.Description,
                        Repository = p.Repository
                    }).First();
            }
        }

        #endregion


        #region Database methods

        protected override int Count(ProjectFilter filter)
        {
            using (var db = new GitTimeContext())
            {
                return db.Projects.Count(filter);
            }
        }

        protected override object Select(int startRow, int endRow, ProjectFilter filter)
        {
            using (var db = new GitTimeContext())
                return db.Projects.SelectFinderRows(startRow, endRow, filter, null);
        }

        protected override SaveResult SaveData(FinderModel model)
        {
            EditorModel editorModel = model.Edit;

            using (var db = new GitTimeContext())
            {
                Project row = editorModel.ID.HasValue
                    ? db.Projects.Find(editorModel.ID.Value)
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

                db.SaveChanges();
            }

            return editorModel.ID.HasValue ? SaveResult.Edited : SaveResult.Added;
        }

        protected override void DeleteData(FinderModel model)
        {
            using (GitTimeContext db = new GitTimeContext())
            {
                db.Projects.Delete(model.Key.Value);
            }
        }

        #endregion
    }
}