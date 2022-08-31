using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.Units;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.SCRObjects;

namespace RealismProjectSCR.SCRObjects.TimeTables
{
    public class Timing 
    {
        public int TimingFrames { get; set; }
        public int Type { get; set; }

        public static readonly int Departure = 0;
        public static readonly int Arrival = 1;
        public static readonly int Depot = 2;
        
        public Timing(int TimingFrames, int Type)
        {
            this.TimingFrames = TimingFrames;
            this.Type = Type;
        }
    }
}
