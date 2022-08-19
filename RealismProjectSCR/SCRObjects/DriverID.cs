using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class DriverID
    {
        public Route Route { get; set; }
        public string PlayerName { get; set; }
        public int DriverNumber { get; set; }
        public string DriverIDString { get; set; }

        public DriverID(Route Route, string PlayerName, int DriverNumber)
        {
            this.Route = Route;
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
