using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitTime.Web.Models.Database
{
    [Table("p.Project")]
    public class Project
    {
        #region Simple properties

        [Key]
        [Column("pk_ID")]
        public Int32 ID { get; set; }

        [Required]
        [Column("fk_CompanyContactID")]
        public Int32 CompanyContactID { get; set; }

        [Required]
        [MaxLength(128)]
        public String Name { get; set; }

        public String Description { get; set; }

        [Required]
        [MaxLength(128)]
        public String Repository { get; set; }

        #endregion

        #region Entities

        [ForeignKey("CompanyContactID")]
        public Company Company { get; set; }

        #endregion
    }
}