using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace AcSwE.Models
{
    [Table("Teacher")]
    public class Teacher
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [MaxLength(255)]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [MaxLength(255)]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "User Name")]
        public string username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Image")]
        public string img { get; set; }

        public string FullName { get { return title + firstName + " " + lastName; } }
        

    }
    public enum setStatus
    {
        Admin,
        Teacher
    }


}