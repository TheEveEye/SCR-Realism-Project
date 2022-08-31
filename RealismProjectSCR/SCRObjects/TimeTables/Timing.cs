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
    public class Timing // This class is not done yet.
    {
        public int TimingFrames { get; set; }
        public int Type { get; set; }
        
        public static int Departure = 0 { get; }
        public static int Arrival = 1 { get; }
        public static int Depot = 2 { get; }
        
        public Timing(int TimingFrames, int Type)
        {
            this.TimingFrames = TimingFrames;
            this.Type = Type;
        }
    }
}
