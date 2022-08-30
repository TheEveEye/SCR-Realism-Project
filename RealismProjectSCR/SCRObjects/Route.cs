using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.Units;
using RealismProjectSCR.NetworkPlanner;

namespace RealismProjectSCR.SCRObjects
{
    public class Route
    {
        public int RouteNumber { get; set; }
        public Station Terminus1 { get; set; }
        public Station Terminus2 { get; set; }
        public Station[] CallingStations { get; set; }
        public string Operator { get; set; }
        public string RoutePath { get; set; }
        public string RouteName { get; set; }

        public Route(int RouteNumber, Station Terminus1, Station Terminus2, Station[] CallingStations, string Operator, string RouteName)
        {
            this.RouteNumber = RouteNumber;
            this.Terminus1 = Terminus1;
            this.Terminus2 = Terminus2;
            this.CallingStations = CallingStations;
            this.Operator = Operator;
            this.RoutePath = Program.ProjectDirectoryPath + @"SCRObjects\Routes\" + this.RouteNumber + ".route";
            this.RouteName = RouteName;
        }
    }
    public struct Operator
    {
        public static int Connect = 0;
        public static int Express = 1;
        public static int Waterline = 2;
        public static int Airlink = 3;
    }
}
