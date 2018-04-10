using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AcSwE.Models
{
    [Table("Location")]
    public class Location
    {
        [Key]
        public int id { get; set; }
                
        [MaxLength(255)]
        [Display(Name = "Location / Place")]
        public string locationName { get; set; }

        [Display(Name = "Building")]
        public string locationPoint { get; set; }
        [Display(Name = "Room")]
        public string room { get; set; }
        
    }
}