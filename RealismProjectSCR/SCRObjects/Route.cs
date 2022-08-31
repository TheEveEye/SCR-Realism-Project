using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.Units;
using RealismProjectSCR.NetworkPlanner;
using RealismProjectSCR;
using RealismProjectSCR.SCRObjects.TimeTables;

namespace RealismProjectSCR.SCRObjects
{
    public class Route
    {
        public static string RoutePath = Program.ProjectDirectoryPath + @"SCRObjects\Routes.SCR";

        public int RouteNumber { get; set; }
        public Station Terminus1 { get; set; }
        public Station Terminus2 { get; set; }
        public Station[] CallingStations { get; set; }
        public Timing[] Timings { get; set; }
        public string Operator { get; set; }
        public string RouteName { get; set; }

        public Route(int RouteNumber, Station Terminus1, Station Terminus2, Station[] CallingStations, Timing[] Timings, string Operator, string RouteName)
        {
            this.RouteNumber = RouteNumber;
            this.Terminus1 = Terminus1;
            this.Terminus2 = Terminus2;
            this.CallingStations = CallingStations;
            this.Timings = Timings;
            this.Operator = Operator;
            this.RouteName = RouteName;
        }
        public static Route[] Import() // This function is not finished. Things to do: CallingStations[] and Timings[] importing. Timings[] will require SCRObjects.Timing Class
        {
            string[] file = File.ReadAllLines(RoutePath);
            Route[] routes = new Route[file.Length];

            for (int i = 0; i < routes.Length; i++)
            {
                string[] tempRoute = file[i].Split(';');
                routes[i].RouteNumber = (i + 1);
                routes[i].RouteName = tempRoute[0];
                routes[i].Operator = tempRoute[1];
                Station[] Terminuses = Station.NamesToStations(tempRoute[2].Split(','));
                routes[i].Terminus1 = Terminuses[0];
                routes[i].Terminus2 = Terminuses[1];
            }
        }
    }
}
