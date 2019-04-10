using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTL.TVMaze.API.Test.Models
{
    public class PagingParameterModel
    {
        public int pageNumber { get; set; } = 1;//default

        public int _pageSize { get; set; } = 10;//default

        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
            }
        }
    }
}  

