using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitTime.Web.Models.Database
{
    [Table("c.Role")]
    public class Role
    {
        #region Simple properties

        [Key]
        [Column("pk_ID")]
        public Int32 ID { get; set; }

        [Required]
        [MaxLength(32)]
        [Index("UNQ_RoleName", IsUnique = true)]
        public String Name { get; set; }

        #endregion

        #region Entities

        public virtual ICollection<Person> Persons { get; set; }

        #endregion

        #region Construction

        public Role()
        {
            Persons = new List<Person>();
        }

        #endregion
    }
}