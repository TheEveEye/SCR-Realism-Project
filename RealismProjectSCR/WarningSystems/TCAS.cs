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

        public enum CollisionStatus
        {
            SameStation = 1,
            SamePlatform = 2,
            SameNextStation = 3,
            SameStationExit = 4,
            SameStationEntrance = 5,
        }

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
        public static List<CollisionStatus> GetCollisionStatus(Departure departure1, Departure departure2)
        {
            List<CollisionStatus> collisionStatuses = new List<CollisionStatus>();

            // Checks if the train is departing at the same station
            if ((departure1.Station.Equals(departure2.Station)) && (TimeFrame.Conflicts(departure1.Occupation, departure2.Occupation)))
            {
                collisionStatuses.Add(CollisionStatus.SameStation);
            }

            // Checks if the trains can depart at the same platform
            // This will not work until Station Platforms are fully added to the program
            /*
            for (int i = 0; i < departure2.PossiblePlatforms.Length; i++)
            {
                if ((departure1.PossiblePlatforms.Contains(departure2.PossiblePlatforms[i])) && (TimeFrame.Conflicts(departure1.Occupation, departure2.Occupation)) && (!collisionStatuses.Contains(CollisionStatus.SamePlatform)))
                {
                    collisionStatuses.Add(CollisionStatus.SamePlatform);
                }
            }
            */

            // Add CollisionStatus.SameNextStation
            // Add CollisionStatus.SameStationEntrance (This will not happen for a while)
            // Add CollisionStatus.SameStationExit (This will not happen for a while)

            return collisionStatuses;
        }
    }
}
