using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RTL.TVMaze.API.Test.Models
{
    public class ShowCastContext : DbContext
    {
        public DbSet<RTL.TVMaze.API.Test.Models.ShowCast> ShowCast { get; set; }

        protected override void OnConfiguring
             (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DST19404;Initial Catalog=RTLFlix;Trusted_Connection=True;");
        }
    }
}
