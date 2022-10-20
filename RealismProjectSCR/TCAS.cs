using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR
{
    public static class TCAS // Traffic Collision Avoidance System
    {
        static int minimumArrivingThreshold = 4;

        static string ThresholdWarning01 = "";


        public static bool DepartureCollision(List<Departure> departures, Departure newDeparture)
        {
            bool output = false;

            for (int i = 0; i < departures.Count; i++)
            {
                if (WillCollide(departures[i], newDeparture))
                {
                    output = true;
                }
            }

            return output;
        }
        public static bool WillCollide(Departure departure1, Departure departure2)
        {
            bool output = false;
            for (int j = 0; j < departure1.PossiblePlatforms.Length; j++)
            {
                if (departure1.PossiblePlatforms.Contains(departure2.PossiblePlatforms[j]))
                {
                    if (TimeFrame.Conflicts(departure1.Occupation, departure2.Occupation))
                    {
                        output = true;
                    }
                }
            }
            return output;
        }
    }
}
