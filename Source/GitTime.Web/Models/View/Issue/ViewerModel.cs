using System;
using System.ComponentModel.DataAnnotations;
using GitTime.Web.Infrastructure.GitHub.Data;

namespace GitTime.Web.Models.View.Issue
{
    public class ViewerModel
    {
        #region Classes

        public class ViewData
        {
            public Int32 ID { get; set; }
            public Int32 Number { get; set; }
            public String Title { get; set; }
            public String BodyText { get; set; }
            public String ActiveTab { get; set; }
            public GitHubCommentInfo[] Comments { get; set; }
            public TimecardData[] Timecards { get; set; }
        }

        public class TimecardData
        {
            public Int32 ID { get; set; }

            [Required]
            public DateTime EntryDate { get; set; }

            [Required]
            [Range(0.01d, 24d, ErrorMessage = "Hours must be greater than zero.")]
            public Decimal Hours { get; set; }

            public Int32 PersonID { get; set; }
            public String PersonFirstName { get; set; }
            public String PersonLastName { get; set; }
        }

        public class IssueHoursData
        {
            public Int32 Number { get; set; }
            public String RepositoryName { get; set; }
            public Decimal Hours { get; set; }
        }

        #endregion

        public virtual ViewData View { get; set; }
        public virtual TimecardData Timecard { get; set; }
    }
}