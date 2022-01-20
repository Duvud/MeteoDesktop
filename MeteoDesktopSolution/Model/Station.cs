using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoDesktopSolution.Model
{
    internal class Station
    {
        public String? id { get; set; }
        public String? name { get; set; }

        public String? nameEus { get; set; }

        public String? municipality { get; set; }

        public String? province { get; set; }

        public int altitude { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        public String stationType { get; set; }

        
    }
}
