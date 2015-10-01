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
        public Int32 ID { get; set; }

        [Column("fk_ProjectID")]
        public Int32 ProjectID { get; set; }

        [Column("fk_PersonContactID")]
        public Int32 PersonContactID { get; set; }

        public Int32? IssueNumber { get; set; }

        public String IssueDescription { get; set; }

        public DateTime EntryDate { get; set; }

        public Decimal Hours { get; set; }

        #endregion

        #region Entities

        [ForeignKey("PersonContactID")]
        public Person Person { get; set; }

        public Project Project { get; set; }

        #endregion
    }
}