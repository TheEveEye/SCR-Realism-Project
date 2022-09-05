using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.NetworkPlanner;
using RealismProjectSCR.Units;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;

namespace RealismProjectSCR.SCRObjects
{
    public class Leg
    {
        public Route Route { get; set; }
        public int StartFrame { get; set; }
        public int EndFrame { get; set; }
        public int TotalFrames { get; set; }
        public Departure[] Departures { get; set; }
        public Station StartingStation { get; set; }
        public Station EndingStation { get; set; }

        public Leg(Route Route, int StartFrame, Departure[] Departures, Station StartingStation, Station EndingStation)
        {
            this.Route = Route;
            this.StartFrame = StartFrame;
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;


        }
    }
}
