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
        public int TotalLength { get; set; }
        
        // Route Change IDs:
        // Used in EditRoute function
        
        public int RouteName = 0; // Change Route Name
        public int RouteOperator = 1; // Change Operator of Route
        public int RouteTerminus = 2; // Change the Terminuses of Route
        public int RouteStationsTimings = 3; // Changes the station and/or Timings of Route

        public Route(int RouteNumber, Station Terminus1, Station Terminus2, Station[] CallingStations, Timing[] Timings, string Operator, string Name, int TotalLength)
        {
            this.RouteNumber = RouteNumber;
            this.Terminus1 = Terminus1;
            this.Terminus2 = Terminus2;
            this.CallingStations = CallingStations;
            this.Timings = Timings;
            this.Operator = Operator;
            this.Name = Name;
            this.TotalLength = TotalLength;
        }

        public static void PrintData(int RouteNumber)
        {
            // Timings for: R001 (Stepford Central <> Airport Central)
            // Station                                   Seconds    Frames    T+
            // Stepford Central (Depart)                 90         4         00:01:30
            // Stepford East                             120        8         00:03:30
            // Stepford High Street                      105        7         00:05:15
            // Whitefield Lido                           75         5         00:06:00
            // Stepford United Football Club (Arrive)    90         6         00:07:30
            // Stepford United Football Club (Depart)    60         4         00:08:30
            // Whitefield Lido                           90         6         00:10:00
            // Stepford High Street                      75         5         00:11:15
            // Stepford East                             105        7         00:13:00
            // Stepford Central (Arrive)                 120        8         00:15:00
            if (!String.IsNullOrEmpty(Program.Routes[RouteNumber - 1].Name))
            {
                Route route = Program.Routes[RouteNumber - 1];
                int LongestStationName = FindLongestStationName(route);
                Console.WriteLine("Timings for: " + RouteNumberString(RouteNumber) + " (" + route.Name + ") Total Time: " + (route.TotalLength * 15 / 60) + " Minutes / " + route.TotalLength + " Frames");
                Console.WriteLine("Station" + Program.IntToSpaces(LongestStationName - 7) + "    Seconds    Frames    T+");
                int totalFrames = 0;
                for (int i = 0; i < route.Timings.Length; i++)
                {
                    int Length = route.Timings[i].Station.Name.ToCharArray().Length;
                    totalFrames += route.Timings[i].TimingFrames;
                    if ((i == 0) || (i == route.Timings.Length / 2))
                    {
                        Console.WriteLine((Program.FillWithSpaces(route.Timings[i].Station.Name + " (Depart)", LongestStationName) + "    " + (route.Timings[i].TimingFrames * 15) + Program.IntToSpaces(7 - (Convert.ToString(route.Timings[i].TimingFrames * 15).ToCharArray().Length)) + "    " + route.Timings[i].TimingFrames + Program.IntToSpaces(6 - (Convert.ToString(route.Timings[i].TimingFrames).ToCharArray().Length)) + "    " + Time.ScheduleFramesToDateTime(totalFrames).ToLongTimeString()));
                    }
                    else if ((i == Program.Routes[RouteNumber - 1].Timings.Length - 1) || (i == route.Timings.Length / 2 - 1))
                    {
                        Console.WriteLine((Program.FillWithSpaces(route.Timings[i].Station.Name + " (Arrive)", LongestStationName) + "    " + (route.Timings[i].TimingFrames * 15) + Program.IntToSpaces(7 - (Convert.ToString(route.Timings[i].TimingFrames * 15).ToCharArray().Length)) + "    " + route.Timings[i].TimingFrames + Program.IntToSpaces(6 - (Convert.ToString(route.Timings[i].TimingFrames).ToCharArray().Length)) + "    " + Time.ScheduleFramesToDateTime(totalFrames).ToLongTimeString()));
                    }
                    else
                    {
                        Console.WriteLine(Program.FillWithSpaces(route.Timings[i].Station.Name, LongestStationName) + "    " + (route.Timings[i].TimingFrames * 15) + Program.IntToSpaces(7 - (Convert.ToString(route.Timings[i].TimingFrames * 15).ToCharArray().Length)) + "    " + route.Timings[i].TimingFrames + Program.IntToSpaces(6 - (Convert.ToString(route.Timings[i].TimingFrames).ToCharArray().Length)) + "    " + Time.ScheduleFramesToDateTime(totalFrames).ToLongTimeString());
                    }
                }
            }
            else
            {
                Console.WriteLine("This Route does not exist. Please try again...");
            }
        }
        
        public static string RouteNumberString(int RouteNumber)
        {
            char[] chars = Convert.ToString(RouteNumber).ToCharArray();
            string output = "";
            switch (chars.Length)
            {
                case 1: output = "R00" + RouteNumber; break;
                case 2: output = "R0" + RouteNumber; break;
                case 3: output = "R" + RouteNumber; break;
                default: output = "0"; break;
            }
            return output;
        }

        public static void EditRoute()
        {

        }

        public static int FindLongestStationName(Route route)
        {
            int Longest = 0;
            if ((route.Terminus1.Name + " (Depart)").ToCharArray().Length > Longest)
            {
                Longest = (route.Terminus1.Name + " (Depart)").ToCharArray().Length;
            }
            if ((route.Terminus2.Name + " (Depart)").ToCharArray().Length > Longest)
            {
                Longest = (route.Terminus2.Name + " (Depart)").ToCharArray().Length;
            }
            foreach (Station station in route.CallingStations)
            {
                if (station.Name.ToCharArray().Length > Longest)
                {
                    Longest = station.Name.ToCharArray().Length;
                }
            }
            return Longest;
        }

        public static Route[] Import() // This function is not finished. Things to do: CallingStations[] and Timings[] importing. Timings[] will require SCRObjects.Timing Class
        {
            string[] file = File.ReadAllLines(RoutePath);
            Route[] routes = new Route[file.Length];

            for (int i = 0; i < routes.Length; i++)
            {
                Route tempRoute = new Route(0, null, null, null, null, null, null, 0);
                string[] tempRouteData = file[i].Split(';'); //Name;TotalLength;Operator;Terminus1,Terminus2;Station1:Timing1,Station2:Timing2
                tempRoute.RouteNumber = (i + 1);
                tempRoute.Name = tempRouteData[0];
                tempRoute.TotalLength = Convert.ToInt32(tempRouteData[1]);
                tempRoute.Operator = tempRouteData[2];
                Station[] Terminuses = Station.NamesToStations(tempRouteData[3].Split(','));
                tempRoute.Terminus1 = Terminuses[0];
                tempRoute.Terminus2 = Terminuses[1];

                string[] StationData = tempRouteData[4].Split(',');
                tempRoute.CallingStations = new Station[StationData.Length];
                tempRoute.Timings = new Timing[StationData.Length];
                for (int j = 0; j < StationData.Length; j++)
                {
                    string[] tempStationData = StationData[j].Split(':');
                    tempRoute.CallingStations[j] = Station.NameToStation(tempStationData[0]);

                    Timing tempTiming = new Timing(0, 3, null);
                    try
                    {
                        tempTiming.TimingFrames = Convert.ToInt32(tempStationData[1]) / 15;
                    }
                    catch (FormatException)
                    {
                        tempTiming.TimingFrames = 0;
                    }
                    
                    tempTiming.Type = Timing.Departure;
                    tempTiming.Station = tempRoute.CallingStations[j];
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
        public string ToExport()
        {
            string output;
            if (CallingStations.Length == 0)
            {
                output = Name + ";" + Convert.ToString(TotalLength) + ";" + Operator + ";" + Terminus1 + "," + Terminus2 + ";" + "" + ":" + "";
            }
            else
            {
                output = Name + ";" + Convert.ToString(TotalLength) + ";" + Operator + ";" + Terminus1.Name + "," + Terminus2.Name + ";" + CallingStations[0].Name + ":" + Timings[0].TimingFrames;
                for (int i = 1; i < CallingStations.Length; i++)
                {
                    output += "," + CallingStations[i].Name + ":" + Timings[i].TimingFrames;
                }
            }
            return output;
            //Display Name;TotalLength;Operator;Terminus 1,Terminus 2;Station1:Time in Frames,Station2:Time in Frames,Station3:Time in Frames,Station4:Time in Frames,Station5:Time in Frames,Station6:Time in Frames
        }
        public static Departure[] GetLegDepartures(Route route, int startingFrame, Station startingStation, Station endingStation)
        {
            int[] startingStationIndexes = GetStationIndexes(route, startingStation);
            int[] endingStationIndexes = GetStationIndexes(route, endingStation);
            if ((startingStationIndexes.Length == 0) || (endingStationIndexes.Length == 0))
            {
                return new Departure[0];
            }
            int[] shortestDistanceIndex = new int[2];
            int shortestDistance = route.CallingStations.Length;
            for (int i = 0; i < startingStationIndexes.Length; i++)
            {
                for (int j = 0; j < endingStationIndexes.Length; j++)
                {
                    if ((endingStationIndexes[j] - startingStationIndexes[i] > 0) && (endingStationIndexes[j] - startingStationIndexes[i] < shortestDistance))
                    {
                        shortestDistance = endingStationIndexes[j] - startingStationIndexes[i];
                        shortestDistanceIndex[0] = startingStationIndexes[i];
                        shortestDistanceIndex[1] = endingStationIndexes[j];
                    }
                }
            }

            Station[] callingLegStations = new Station[shortestDistance + 1];
            for (int i = shortestDistanceIndex[0]; i < shortestDistanceIndex[1] + 1; i++)
            {
                callingLegStations[i - shortestDistanceIndex[0]] = route.CallingStations[i];
            }

            Departure[] legDepartures = new Departure[callingLegStations.Length];
            int timer = startingFrame;
            for (int i = 0; i < legDepartures.Length; i++)
            {
                timer += route.Timings[i + shortestDistanceIndex[0]].TimingFrames;
                legDepartures[i] = new Departure(timer, callingLegStations[i], null, endingStation, route);
            }

            return legDepartures;
        }
        public static int[] GetStationIndexes(Route route, Station station)
        {
            List<int> output = new List<int>();
            for (int i = 0; i < route.CallingStations.Length; i++)
            {
                if (route.CallingStations[i] == station)
                {
                    output.Add(i);
                }
            }
            return output.ToArray();
        }

        public static void Push(Route[] routes)
        {
            string[] exportRoutes = new string[routes.Length];
            for (int i = 0; i < routes.Length; i++)
            {
                exportRoutes[i] = routes[i].ToExport();
            }
            File.WriteAllLines(RoutePath, exportRoutes);
        }
    }
}
