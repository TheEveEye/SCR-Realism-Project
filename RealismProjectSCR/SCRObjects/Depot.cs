using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class Depot
    {
        public string Name { get; set; }
        public int DepotID { get; set; }
        public Station[] AdjacentStations { get; set; }

    }
}
