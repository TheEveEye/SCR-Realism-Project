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
        public string Name { get; set; }
        
        // Route Change IDs:
        // Used in EditRoute function
        
        public int RouteName = 0; // Change Route Name
        public int RouteOperator = 1; // Change Operator of Route
        public int RouteTerminus = 2; // Change the Terminuses of Route
        public int RouteStationsTimings = 3; // Changes the station and/or Timings of Route

        public Route(int RouteNumber, Station Terminus1, Station Terminus2, Station[] CallingStations, Timing[] Timings, string Operator, string Name)
        {
            this.RouteNumber = RouteNumber;
            this.Terminus1 = Terminus1;
            this.Terminus2 = Terminus2;
            this.CallingStations = CallingStations;
            this.Timings = Timings;
            this.Operator = Operator;
            this.Name = Name;
        }
        public static Route[] Import() // This function is not finished. Things to do: CallingStations[] and Timings[] importing. Timings[] will require SCRObjects.Timing Class
        {
            string[] file = File.ReadAllLines(RoutePath);
            Route[] routes = new Route[file.Length];

            for (int i = 0; i < routes.Length; i++)
            {
                Route tempRoute = new Route(0, null, null, null, null, null, null);
                string[] tempRouteData = file[i].Split(';');
                tempRoute.RouteNumber = (i + 1);
                tempRoute.Name = tempRouteData[0];
                tempRoute.Operator = tempRouteData[1];
                Station[] Terminuses = Station.NamesToStations(tempRouteData[2].Split(','));
                tempRoute.Terminus1 = Terminuses[0];
                tempRoute.Terminus2 = Terminuses[1];

                string[] StationData = tempRouteData[3].Split(',');
                tempRoute.CallingStations = new Station[StationData.Length];
                tempRoute.Timings = new Timing[StationData.Length];
                for (int j = 0; j < StationData.Length; j++)
                {
                    string[] tempStationData = StationData[j].Split(':');
                    tempRoute.CallingStations[j] = Station.NameToStation(tempStationData[0]);

                    Timing tempTiming = new Timing(Convert.ToInt32(tempStationData[1]), Timing.Departure, tempRoute.CallingStations[j]);
                    if ((j == StationData.Length / 2) || (j == StationData.Length - 1))
                    {
                        tempTiming.Type = Timing.Arrival;   
                    }
                    tempRoute.Timings[j] = tempTiming;
                }
                routes[i] = tempRoute;
            }
            return routes;
        }
        
        public static void EditRoute()
        {
            
        }
    }
}
