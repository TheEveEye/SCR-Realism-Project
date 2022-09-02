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
                string[] tempRoute = file[i].Split(';');
                routes[i].RouteNumber = (i + 1);
                routes[i].Name = tempRoute[0];
                routes[i].Operator = tempRoute[1];
                Station[] Terminuses = Station.NamesToStations(tempRoute[2].Split(','));
                routes[i].Terminus1 = Terminuses[0];
                routes[i].Terminus2 = Terminuses[1];

                string[] StationData = tempRoute[3].Split(',');
                routes[i].CallingStations = new Station[StationData.Length];
                routes[i].Timings = new Timing[StationData.Length];
                for (int j = 0; j < StationData.Length; j++)
                {
                    string[] tempStationData = StationData[j].Split(':');
                    routes[i].CallingStations[j] = Station.NameToStation(tempStationData[0]);

                    Timing tempTiming = new Timing(Convert.ToInt32(tempStationData[1]), Timing.Departure, routes[i].CallingStations[j]);
                    if ((j == StationData.Length / 2) || (j == StationData.Length - 1))
                    {
                        tempTiming.Type = Timing.Arrival;   
                    }
                    routes[i].Timings[j] = tempTiming;
                }
            }
        }
        
        public static void EditRoute()
        {
            
        }
    }
}
