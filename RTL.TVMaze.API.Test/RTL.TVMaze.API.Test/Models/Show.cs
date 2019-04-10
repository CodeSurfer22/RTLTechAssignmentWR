﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RTL.TVMaze.API.Test.Models
{
    public class Show
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowID { get; set; }
        public string ShowName { get; set; }
    }
}