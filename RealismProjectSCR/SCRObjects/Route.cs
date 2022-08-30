using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.Units;
using RealismProjectSCR.NetworkPlanner;
using RealismProjectSCR;

namespace RealismProjectSCR.SCRObjects
{
    public class Route
    {
        public static string RoutePath = Program.ProjectDirectoryPath + @"SCRObjects\Routes.SCR";

        public int RouteNumber { get; set; }
        public Station Terminus1 { get; set; }
        public Station Terminus2 { get; set; }
        public Station[] CallingStations { get; set; }
        public string Operator { get; set; }
        public string RouteName { get; set; }

        public Route(int RouteNumber, Station Terminus1, Station Terminus2, Station[] CallingStations, string Operator, string RouteName)
        {
            this.RouteNumber = RouteNumber;
            this.Terminus1 = Terminus1;
            this.Terminus2 = Terminus2;
            this.CallingStations = CallingStations;
            this.Operator = Operator;
            this.RouteName = RouteName;
        }
        public static Route[] Import() // Requieres Timing class to be finished.
        {
            string[] file = File.ReadAllLines(RoutePath);
            Route[] routes = new Route[file.Length];

            for (int i = 0; i < routes.Length; i++)
            {
                string[] tempRoute = file[i].Split(';');

            }
        }
    }
}
