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
        public static string RoutePath = Program.ProjectDirectoryPath + @"SCRObjects\Routes.txt";

        public int RouteNumber { get; set; }
        public Station Terminus1 { get; set; }
        public Station Terminus2 { get; set; }
        public Station[] CallingStations { get; set; }
        public Timing[] Timings { get; set; }
        public Operator Operator { get; set; }
        public int HeadcodePriority { get; set; }
        public string Name { get; set; }
        public int TotalLength { get; set; }

        public Route(int RouteNumber, Station Terminus1, Station Terminus2, Station[] CallingStations, Timing[] Timings, Operator Operator, int HeadcodePriority, string Name, int TotalLength)
        {
            this.RouteNumber = RouteNumber;
            this.Terminus1 = Terminus1;
            this.Terminus2 = Terminus2;
            this.CallingStations = CallingStations;
            this.Timings = Timings;
            this.Operator = Operator;
            this.HeadcodePriority = HeadcodePriority;
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
            try
            {
                if (!String.IsNullOrEmpty(Program.Routes[RouteNumber - 1].Name))
                {
                    Route route = Program.Routes[RouteNumber - 1];
                    int LongestStationName = FindLongestStationName(route);
                    Console.WriteLine("Timings for: " + RouteNumberString(RouteNumber) + " (" + route.Name + ") Total Time: " + (route.TotalLength * 15 / 60) + " Minutes / " + route.TotalLength + " Frames");
                    Console.WriteLine("Station" + Program.IntToSpaces(LongestStationName - 7) + "    Seconds    Frames    T+");
                    int totalFrames = 0;
                    for (int i = 0; i < route.Timings.Length; i++)
                    {
                        int Length = route.Timings[i].Station.Name.Length;
                        totalFrames += route.Timings[i].TimingFrames;
                        if ((i == 0) || (i == route.Timings.Length / 2))
                        {
                            Console.WriteLine((Program.FillWithSpaces(route.Timings[i].Station.Name + " (Depart)", LongestStationName) + "    " + (route.Timings[i].TimingFrames * 15) + Program.IntToSpaces(7 - (Convert.ToString(route.Timings[i].TimingFrames * 15).Length)) + "    " + route.Timings[i].TimingFrames + Program.IntToSpaces(6 - (Convert.ToString(route.Timings[i].TimingFrames).Length)) + "    " + Time.ScheduleFramesToDateTime(totalFrames).ToLongTimeString()));
                        }
                        else if ((i == Program.Routes[RouteNumber - 1].Timings.Length - 1) || (i == route.Timings.Length / 2 - 1))
                        {
                            Console.WriteLine((Program.FillWithSpaces(route.Timings[i].Station.Name + " (Arrive)", LongestStationName) + "    " + (route.Timings[i].TimingFrames * 15) + Program.IntToSpaces(7 - (Convert.ToString(route.Timings[i].TimingFrames * 15).Length)) + "    " + route.Timings[i].TimingFrames + Program.IntToSpaces(6 - (Convert.ToString(route.Timings[i].TimingFrames).Length)) + "    " + Time.ScheduleFramesToDateTime(totalFrames).ToLongTimeString()));
                        }
                        else
                        {
                            Console.WriteLine(Program.FillWithSpaces(route.Timings[i].Station.Name, LongestStationName) + "    " + (route.Timings[i].TimingFrames * 15) + Program.IntToSpaces(7 - (Convert.ToString(route.Timings[i].TimingFrames * 15).Length)) + "    " + route.Timings[i].TimingFrames + Program.IntToSpaces(6 - (Convert.ToString(route.Timings[i].TimingFrames).Length)) + "    " + Time.ScheduleFramesToDateTime(totalFrames).ToLongTimeString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("This Route does not exist. Please try again...");
                }
            }
            catch (Exception)
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
        public string ToNumberNameOutput()
        {
            return String.Format("{0} ({1})", Route.RouteNumberString(this.RouteNumber), this.Name);
        }
        public void EditRoute(string whatShallBeChanged, string newValue)
        {
            /*
            string[] modifyName = {"Name", "RouteName"};
            string[] modifyOperator = {"Operator"};
            string[] modifyTerminus1 = {"Terminus1"};
            string[] modifyTerminus2 = {"Terminus2"};
            string[] modifyStations = {"Stations", "Stops"};
            string[] modifyTimings = {"Timings", "Departures"};
            string[] modifyHeadcodePriority = {"Headcode", "Priority", "HeadcodePriority"};
            */
            switch (whatShallBeChanged)
            {
                case "name":
                case "routename":
                    this.Name = newValue;
                    break;

                case "operator":
                    try
                    {
                        this.Operator = Operator.FromString(newValue);
                    }
                    catch (ArgumentException)
                    {
                        throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                    }
                    break;

                case "terminus1":
                    try
                    {
                        Station newStation = Station.NameToStation(newValue);
                        if (Station.TerminusStations.Contains(newStation))
                        {
                            this.Terminus1 = newStation;
                        }
                        else
                        {
                            throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                        }
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                    }
                    break;

                case "terminus2":
                    try
                    {
                        Station newStation = Station.NameToStation(newValue);
                        if (Station.TerminusStations.Contains(newStation))
                        {
                            this.Terminus2 = newStation;
                        }
                        else
                        {
                            throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                        }
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                    }
                    break;

                case "headcode":
                case "priority":
                    try
                    {
                        int newHeadcode = Convert.ToInt32(newValue);
                        if ((newHeadcode == 9) || (newHeadcode == 1) || (newHeadcode == 2))
                        {
                            this.HeadcodePriority = newHeadcode;
                        }
                        else
                        {
                            throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                        }
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException(String.Format("\"{0}\" is not a valid value to change.", newValue));
                    }

                    break;

                default:
                    throw new ArgumentException(String.Format("\"{0}\" is not a valid object to change.", whatShallBeChanged));
            }
        }

        public static int FindLongestStationName(Route route)
        {
            int Longest = 0;
            if ((route.Terminus1.Name + " (Depart)").Length > Longest)
            {
                Longest = (route.Terminus1.Name + " (Depart)").Length;
            }
            if ((route.Terminus2.Name + " (Depart)").Length > Longest)
            {
                Longest = (route.Terminus2.Name + " (Depart)").Length;
            }
            foreach (Station station in route.CallingStations)
            {
                if (station.Name.Length > Longest)
                {
                    Longest = station.Name.Length;
                }
            }
            return Longest;
        }

        public static Route[] Import() // Adding in Platform Allocations soon.
        {
            string[] file = File.ReadAllLines(RoutePath);
            Route[] routes = new Route[file.Length];

            for (int i = 0; i < routes.Length; i++)
            {
                Route tempRoute = new Route(0, null, null, null, null, null, 0, null, 0);
                string[] tempRouteData = file[i].Split(';'); //Name;TotalLength;Operator;Terminus1,Terminus2;Station1:Timing1,Station2:Timing2 !!! THIS WILL CHANGE ONCE PLATFORM ALLOCATIONS ARE ADDED !!!
                tempRoute.RouteNumber = (i + 1);
                tempRoute.Name = tempRouteData[0];
                tempRoute.TotalLength = Convert.ToInt32(tempRouteData[1]);
                try
                {
                    tempRoute.Operator = Operator.FromString(tempRouteData[2]);
                }
                catch (ArgumentException)
                {
                    tempRoute.Operator = routes[i - 1].Operator;
                }
                try
                {
                    tempRoute.HeadcodePriority = Convert.ToInt32(tempRouteData[3]);
                }
                catch (Exception)
                {
                    
                }
                Station[] Terminuses = Station.NamesToStations(tempRouteData[4].Split(','));
                tempRoute.Terminus1 = Terminuses[0];
                tempRoute.Terminus2 = Terminuses[1];

                string[] StationData = tempRouteData[5].Split(',');
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
