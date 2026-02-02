using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr14
{
    public static class Core
    {
        public static RDCinemaEntities Context = new RDCinemaEntities();
        public static Users CurrentUser { get; set; }
    }
}