using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitTime.Web.Models.Database
{
    [Table("c.Contact")]
    public class Contact
    {
        [Key]
        [Column("pk_ID")]
        public Int32 ID { get; set; }
    }
}