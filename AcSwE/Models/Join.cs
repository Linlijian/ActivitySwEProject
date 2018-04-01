using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcSwE.Models
{
    [Table("Join")]
    public class Join
    {
        [Key]
        public int id { get; set; }


        public int idStd { get; set; }
        public int idTea { get; set; }
        public int idActivity { get; set; }
    }
}