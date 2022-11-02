using RealismProjectSCR.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class Driver
    {
        public Route Route { get; set; }
        public int SpawningFrame { get; set; }
        public List<Leg> Legs { get; set; }
        public string PlayerName { get; set; }

        public Driver(Route Route, int SpawningFrame, List<Leg> Legs, string PlayerName)
        {
            this.Route = Route;
            this.SpawningFrame = SpawningFrame;
            this.Legs = Legs;
            this.PlayerName = PlayerName;
        }

        public static string[] ToCompacts(Driver[] drivers)
        {
            string[] output = new string[drivers.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = String.Format("{0}: {1}   Route: {2}    {3} Legs    Spawn Time: {4}", i, drivers[i].PlayerName, drivers[i].Route.ToNumberNameOutput(), drivers[i].Legs.Count, Time.ScheduleFramesToDateTime(drivers[i].SpawningFrame).ToLongTimeString());
            }
            return output;
        }
    }
}
