using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RTL.TVMaze.API.Test.Models
{
    public class ShowCast
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowCastID { get; set; }
        public int ShowID { get; set; }
        public int CastID { get; set; }
        
    }
}
