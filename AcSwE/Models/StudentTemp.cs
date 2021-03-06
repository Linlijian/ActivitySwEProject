﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AcSwE.Models
{
    [Table("StudentTemp")]
    public class StudentTemp
    {
        [Key]
        public int id { get; set; }

        public string title { get; set; }

        [Required]
        public int idStd { get; set; }


        [MaxLength(255)]
        public string firstName { get; set; }

        [MaxLength(255)]
        public string lastName { get; set; }

        public string year { get; set; }
        public string img { get; set; }
        public static IEnumerable<DataRow> Rows { get; internal set; }
    }
}