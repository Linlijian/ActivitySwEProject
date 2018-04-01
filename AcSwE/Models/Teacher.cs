﻿using System;
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

        public string title { get; set; }

        [MaxLength(255)]
        public string firstName { get; set; }

        [MaxLength(255)]
        public string lastName { get; set; }

        public string username { get; set; }

        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        public string status { get; set; }

        public string img { get; set; }
        

    }
    public enum setStatus
    {
        Admin,
        Teacher
    }


}