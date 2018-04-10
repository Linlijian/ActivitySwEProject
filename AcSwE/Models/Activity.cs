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
        [Display (Name = "Activity Name")]
        [MaxLength(255)]
        public string activityname { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Location / Place")]
        public string location { get; set; }

        [Display(Name = "Building")]
        public string locationPoint { get; set; }
        [Display(Name = "Room")]
        public string room { get; set; }

        [Required]
        [Display(Name = "Adviser / Teacher")]
        public int teacherInActivity { get; set; }

        [Display(Name = "Year of student")]
        public int yearStd { get; set; }
        [Display(Name = "Year of study")]
        public int yearStudy { get; set; }
        [Display(Name = "Start date")]
        public string startDate { get; set; }
        [Display(Name = "End date")]
        public string endDate { get; set; }
        [Display(Name = "Image")]
        public string img { get; set; }

        [NotMapped]
        public List<Teacher> TeacherList { get; set; }
    }
}