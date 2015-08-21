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
        public int ID { get; set; }

        [Required]
        [Column("fk_CompanyContactID")]
        public int CompanyContactID { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [MaxLength(128)]
        public string Repository { get; set; }

        #endregion

        #region Entities

        [ForeignKey("CompanyContactID")]
        public Company Company { get; set; }

        #endregion
    }
}