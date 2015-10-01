using System;

using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Timecard
{
    public class EditorModel
    {
        public Int32? ID { get; set; }

        public DateTime EntryDate { get; set; }

        [Required]
        public Int32? PersonContactID { get; set; }

        [Required]
        public Int32? ProjectID { get; set; }

        public Int32? IssueNumber { get; set; }

        public String IssueDescription { get; set; }

        [Required]
        public Decimal? Hours { get; set; }
    }
}