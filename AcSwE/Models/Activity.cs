using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AcSwE.Models
{
    [Table("Activity")]
    public class Activity
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(255)]
        public string activityname { get; set; }

        [Required]
        [MaxLength(255)]
        public string location { get; set; }

        [Required]
        public int teacherInActivity { get; set; }

        public int yearStd { get; set; }
        public int yearStudy { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string img { get; set; }

    }
}