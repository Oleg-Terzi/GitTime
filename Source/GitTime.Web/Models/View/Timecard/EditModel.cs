using System;
using System.ComponentModel.DataAnnotations;

namespace GitTime.Web.Models.View.Timecard
{
    public class EditModel
    {
        public int? ID { get; set; }

        public DateTime EntryDate { get; set; }

        [Required]
        public string PersonFullName { get; set; }

        [Required]
        public string ProjectName { get; set; }

        public int? IssueNumber { get; set; }

        public string IssueDescription { get; set; }

        [Required]
        public decimal? Hours { get; set; }
    }
}