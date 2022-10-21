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
        public int DriverNumber { get; set; }
        public string DriverIDString { get; set; }

        public Driver(Route Route, int SpawningFrame, List<Leg> Legs, string PlayerName, int DriverNumber)
        {
            this.Route = Route;
            this.SpawningFrame = SpawningFrame;
            this.Legs = Legs;
            this.PlayerName = PlayerName;
            this.DriverNumber = DriverNumber;
            this.DriverIDString = this.ToStringID();
        }
        private string ToStringID()
        {
            return Route.RouteNumber + "." + DriverNumber;
        }
    }
}
