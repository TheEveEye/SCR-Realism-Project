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
        public Type TimingType { get; set; }
        public Station Station { get; set; }

        public enum Type
        {
            Departure = 0,
            Arrival = 1,
            Depot = 2,
            Invalid = 3
        }
        
        public Timing(int TimingFrames, Type TimingType, Station Station)
        {
            this.TimingFrames = TimingFrames;
            this.TimingType = TimingType;
            this.Station = Station;
        }
    }
}
