﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatDatabase.Data.Model
{
    public class ProjectTaskStatus
    {
        [Key]
        public int ID { get; set; }
        public string Status { get; set; }
    }
}
