using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RTL.TVMaze.API.Test.Models
{
    public class Cast
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CastID { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDay { get; set; }
    }
}
