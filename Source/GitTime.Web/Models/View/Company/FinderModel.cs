namespace GitTime.Web.Models.View.Company
{
    public class FinderModel
    {
        public int? Key { get; set; }

        public virtual BaseSearchResultsModel SearchResults { get; set; }
        public virtual SearchCriteriaModel SearchCriteria { get; set; }
        public virtual EditorModel Edit { get; set; }
    }
}