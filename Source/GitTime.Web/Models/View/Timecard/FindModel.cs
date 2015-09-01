using GitTime.Web.Models.View;

namespace GitTime.Web.Models.View.Timecard
{
    public class FindModel
    {
        public int? Key { get; set; }

        public virtual BaseSearchResultsModel SearchResults { get; set; }
        public virtual SearchCriteriaModel SearchCriteria { get; set; }
        public virtual EditModel Edit { get; set; }
    }
}