using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RTL.TVMaze.API.Test.Models
{
    public class ShowWithCast
    {
        public int ShowID { get; set; }
        public string Name { get; set; }
        public List<Cast> Cast{ get; set; }

    }
}
