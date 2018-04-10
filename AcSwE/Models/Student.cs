using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AcSwE.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [Required]
        [Display(Name = "Id Student")]
        public int idStd { get; set; }


        [MaxLength(255)]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [MaxLength(255)]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Year of student")]
        public string year { get; set; }
        [Display(Name = "Image")]
        public string img { get; set; }
        public static IEnumerable<DataRow> Rows { get; internal set; }
    }
}