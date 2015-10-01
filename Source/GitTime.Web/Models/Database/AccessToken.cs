using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitTime.Web.Models.Database
{
    [Table("c.AccessToken")]
    public class AccessToken
    {
        #region Simple properties

        [Key]
        [Column("pk_ID")]
        public Int32 ID { get; set; }

        [Column("fk_ContactID")]
        [ForeignKey("Contact")]
        [Index("UNQ_AccessToken", 1, IsUnique = true)]
        public Int32 ContactID { get; set; }

        [Required]
        [MaxLength(16)]
        [Index("UNQ_AccessToken", 2, IsUnique = true)]
        public String Application { get; set; }

        [Required]
        public String Key { get; set; }

        [Required]
        public DateTime UtcCreated { get; set; }

        #endregion

        #region Entities
        
        public Contact Contact { get; set; }

        #endregion
    }
}