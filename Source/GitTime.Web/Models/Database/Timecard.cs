using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitTime.Web.Models.Database
{
    [Table("t.Timecard")]
    public class Timecard
    {
        #region Simple properties

        [Key]
        [Column("pk_ID")]
        public int ID { get; set; }

        [Column("fk_ProjectID")]
        public int ProjectID { get; set; }

        [Column("fk_PersonContactID")]
        public int PersonContactID { get; set; }

        public int? IssueNumber { get; set; }

        public string IssueDescription { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal Hours { get; set; }

        #endregion

        #region Entities

        [ForeignKey("PersonContactID")]
        public Person Person { get; set; }

        public Project Project { get; set; }

        #endregion
    }
}