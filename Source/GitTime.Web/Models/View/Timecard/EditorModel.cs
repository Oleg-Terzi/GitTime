using System;

using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Timecard
{
    public class EditorModel
    {
        public int? ID { get; set; }

        public DateTime EntryDate { get; set; }

        [Required]
        public int? PersonContactID { get; set; }

        [Required]
        public int? ProjectID { get; set; }

        public int? IssueNumber { get; set; }

        public string IssueDescription { get; set; }

        [Required]
        public decimal? Hours { get; set; }
    }
}