using RealismProjectSCR.SCRObjects.TimeTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR
{
    public static class TCAS // Train Collision Avoidance System
    {
        static int minimumArrivingThreshold = 4;

        static string ThresholdWarning01 = "";


        public static bool DepartureCollision(List<Departure> departures, Departure newDeparture)
        {

            List<int> departureFrames = new List<int>();
            for (int i = 0; i < departures.Count; i++)
            {
                departureFrames.Add(departures[i].Frame);
            }
            departureFrames.Sort();


        }
        public static bool WillCollide(Departure departure1, Departure departure2)
        {
            if ()
            {

            }
        }
    }
}
