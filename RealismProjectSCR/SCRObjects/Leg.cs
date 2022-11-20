using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.Units;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;

namespace RealismProjectSCR.SCRObjects
{
    public class Leg
    {
        public Route Route { get; set; }
        public TimeFrame TimeFrame { get; set; }
        public int TotalFrames { get; set; }
        public Departure[] Departures { get; set; }
        public Station StartingStation { get; set; }
        public Station EndingStation { get; set; }
        public Headcode? Headcode { get; set; }
        public Driver Driver { get; set; }

        public DateTime? CreationTime { get; set; }

        public Leg(Route Route, int StartFrame, int EndFrame, Departure[] Departures, Station StartingStation, Station EndingStation, Headcode? Headcode, Driver Driver, DateTime? CreationTime = null) //Constructor with startFrame, endFrame and Headcode
        {
            this.Route = Route;
            this.TimeFrame = new(StartFrame, EndFrame);
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
            this.Headcode = Headcode;
            this.Driver = Driver;

            this.CreationTime = CreationTime;
        }
        public Leg(Route Route, TimeFrame TimeFrame, Departure[] Departures, Station StartingStation, Station EndingStation, Headcode? Headcode, Driver Driver, DateTime? CreationTime = null) //Constructor with TimeFrame and Headcode
        {
            this.Route = Route;
            this.TimeFrame = TimeFrame;
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
            this.Headcode = Headcode;
            this.Driver = Driver;

            this.CreationTime = CreationTime;
        }
        public Leg(Route Route, int StartFrame, int EndFrame, Departure[] Departures, Station StartingStation, Station EndingStation, Driver Driver, DateTime? CreationTime = null) //Constructor with startFrame and endFrame
        {
            this.Route = Route;
            this.TimeFrame = new(StartFrame, EndFrame);
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
            this.Driver = Driver;

            this.CreationTime = CreationTime;
        }
        public Leg(Route Route, TimeFrame TimeFrame, Departure[] Departures, Station StartingStation, Station EndingStation, Driver Driver, DateTime? CreationTime = null) //Constructor with TimeFrame
        {
            this.Route = Route;
            this.TimeFrame = TimeFrame;
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
            this.Driver = Driver;

            this.CreationTime = CreationTime;
        }
        public static Leg Create(Route route, int startingFrame, Station startingStation, Station endingStation, Driver driver)
        {
            Leg output = new Leg(route, new(0, 0), null, startingStation, endingStation, driver, null);
            output.Departures = Route.GetLegDepartures(route, startingFrame, startingStation, endingStation);
            output.TimeFrame = DeparturesTimeFrame(output.Departures);

            output.CreationTime = DateTime.UtcNow; 

            return output;
        }

        public Station GetTerminus()
        {
            if (Route.IsTerminus(EndingStation))
            {
                return EndingStation;
            }
            else
            {
                if (Route.Terminus1 == StartingStation)
                {
                    return Route.Terminus2;
                }
                else if (Route.Terminus2 == StartingStation)
                {
                    return Route.Terminus1;
                }
                else
                {
                    return null;
                }
            }
        }
        public enum SortType
        {
            StartTime = 1,
            EndTime = 2,
            Route = 3,
        }
        public static List<Leg> Sort(List<Leg> input, SortType type)
        {
            List<Leg> output = new List<Leg>();
            switch (type)
            {
                case SortType.StartTime: // Sorts Legs by starting time
                    List<int> startingTimes = new List<int>();
                    List<int> sortedStartingTimes = new List<int>();
                    for (int i = 0; i < input.Count; i++)
                    {
                        startingTimes.Add(input[i].TimeFrame.Start);
                        sortedStartingTimes.Add(input[i].TimeFrame.Start);
                    }
                    sortedStartingTimes.Sort();
                    for (int i = 0; i < input.Count; i++)
                    {
                        output.Add(input[Array.IndexOf(startingTimes.ToArray(), sortedStartingTimes[i])]);
                    }
                    break;

                case SortType.EndTime: // Sorts Legs by ending time
                    List<int> endingTimes = new List<int>();
                    List<int> sortedEndingTimes = new List<int>();
                    for (int i = 0; i < input.Count; i++)
                    {
                        endingTimes.Add(input[i].TimeFrame.Start);
                        sortedEndingTimes.Add(input[i].TimeFrame.Start);
                    }
                    sortedEndingTimes.Sort();
                    for (int i = 0; i < input.Count; i++)
                    {
                        output.Add(input[Array.IndexOf(endingTimes.ToArray(), sortedEndingTimes[i])]);
                    }
                    break;

                case SortType.Route: // Sorts Legs by route number
                    List<int> routeNumbers = new List<int>();
                    List<int> sortedRouteNumbers = new List<int>();
                    for (int i = 0; i < input.Count; i++)
                    {
                        routeNumbers.Add(input[i].Route.RouteNumber);
                        sortedRouteNumbers.Add(input[i].Route.RouteNumber);
                    }
                    sortedRouteNumbers.Sort();
                    for (int i = 0; i < input.Count; i++)
                    {
                        output.Add(input[Array.IndexOf(routeNumbers.ToArray(), sortedRouteNumbers[i])]);
                    }
                    break;

                default:
                    output = input;
                    break;
            }
            return output;
        }

        public static TimeFrame DeparturesTimeFrame(Departure[] departures)
        {
            return new TimeFrame(departures[0].Frame, departures[departures.Length - 1].Frame);
        }

        public string ToDriver(int legNumber = 0)
        {
            List<string> output = new List<string>();
            if (legNumber == 0)
            {
                output.Add("Departures for Leg " + (Array.IndexOf(Program.ActiveShift.Legs.ToArray(), this) + 1));
            }
            else
            {
                output.Add("Departures for Leg " + legNumber);
            }
            if (this.Headcode == null)
            {
                output.Add("Starting Time: " + TimeFrame.StartTime + "    Route: " + Route.RouteNumberString(Route.RouteNumber) + " (" + Route.Name + ")    Last Station: " + Departures[Departures.Length - 1].Station.Name);
            }
            else
            {
                output.Add("Starting Time: " + TimeFrame.StartTime + "    Route: " + Route.RouteNumberString(Route.RouteNumber) + " (" + Route.Name + ")    Last Station: " + Departures[Departures.Length - 1].Station.Name + "    Predicted Headcode: " + Headcode.Code);
            }
            output.Add("");
            foreach (var item in this.Departures)
            {
                output.Add(item.ToDriver());
            }
            
            // 12:25:00 + Stepford Central
            //          |
            // 12:27:00 + Stepford East
            //          |
            // 12:28:45 + Stepford High Street
            
            string outputString = "";
            foreach (string Line in output) // Building String
            {
                outputString += Line + "\n";
            }
            
            return outputString;
        }

        public static string ToExport(Leg leg)
        {
            string output = Convert.ToString(leg.Route.RouteNumber) + ";" + Array.IndexOf(Program.ActiveShift.Drivers.ToArray(), leg.Driver) + ";" + Convert.ToString(leg.TimeFrame.Start) + "," + Convert.ToString(leg.TimeFrame.End) + ";" + leg.StartingStation.Name + "," + leg.EndingStation.Name + ";" + leg.Departures[0].Station.Name + ":" + Convert.ToString(leg.Departures[0].Frame);
            for (int i = 1; i < leg.Departures.Length; i++)
            {
                output += "," + leg.Departures[i].Station.Name + ":" + Convert.ToString(leg.Departures[i].Frame);
            }
            return output;
        }

        public string ToCompact(int LegNumber, string indexSeperator = "    ")
        {
            if (this.Headcode == null)
            {
                return "Leg " + LegNumber + "    Route: " + Route.RouteNumberString(this.Route.RouteNumber) + " (" + this.Route.Name + ")    Time Frame: " + this.TimeFrame.StartTime + " - " + this.TimeFrame.EndTime + " (" + this.TimeFrame.Start + " - " + this.TimeFrame.End + ")    Driving: " + this.StartingStation.Name + " - " + this.EndingStation.Name;
            }
            else
            {
                return "Leg " + LegNumber + "    Route: " + Route.RouteNumberString(this.Route.RouteNumber) + " (" + this.Route.Name + ")    Time Frame: " + this.TimeFrame.StartTime + " - " + this.TimeFrame.EndTime + " (" + this.TimeFrame.Start + " - " + this.TimeFrame.End + ")    Driving: " + this.StartingStation.Name + " - " + this.EndingStation.Name + "    Predicted Headcode: " + this.Headcode.Code;
            }
        }

        public static Leg CreateInteractive(string[] args)
        {
            Leg output = new Leg(null, new TimeFrame(0, 0), null, null, null, null, DateTime.UtcNow);
            
            // Collect DriverID, Route, Starting Station, Ending Station, First Station Start
            // Don't forget to include TCAS, Driver System
            // Eventually add automatically created timings based on frequencies
            // Throw an exception with error message
            bool cancelled = false;
            /*
             
             
             
            */
            Console.WriteLine("Creating new leg...");
            output.Driver = Driver.GetDriverInteractive(true); // Gets driver
            output.Route = Route.GetRouteInteractive(output.Driver, true); // Gets Route

            return output;
        }

        public static Leg LastCreatedLeg()
        {
            List<Leg> legs = Program.ActiveShift.Legs;
            List<long> creationUnixes = new List<long>();
            List<long> sortedCreationUnixes = new List<long>();
            for (int i = 0; i < legs.Count; i++)
            {
                creationUnixes[i] = new DateTimeOffset(Convert.ToDateTime(legs[i].CreationTime)).ToUnixTimeSeconds();
                sortedCreationUnixes[i] = new DateTimeOffset(Convert.ToDateTime(legs[i].CreationTime)).ToUnixTimeSeconds();
            }
            sortedCreationUnixes.Sort();
            return legs[Array.IndexOf(creationUnixes.ToArray(), sortedCreationUnixes[sortedCreationUnixes.Count - 1])];
        }

        public static Leg Import(string input, List<Driver> drivers)
        {
            string[] split = input.Split(';');
            Leg output = new Leg(null, 0, 0, new Departure[0], null, null, null, null);
            output.Route = Program.Routes[Convert.ToInt32(split[0]) - 1];
            output.Driver = drivers[Convert.ToInt32(split[1])];
            string[] timeframe = split[2].Split(',');
            output.TimeFrame = new(Convert.ToInt32(timeframe[0]), Convert.ToInt32(timeframe[1]));
            output.TotalFrames = output.TimeFrame.ToFrames();
            string[] terminus = split[3].Split(',');
            output.StartingStation = Station.NameToStation(terminus[0]);
            output.EndingStation = Station.NameToStation(terminus[1]);

            string[] rawdepartures = split[4].Split(',');
            Departure[] departures = new Departure[rawdepartures.Length];
            for (int i = 0; i < departures.Length; i++)
            {
                string[] rawdeparture = rawdepartures[i].Split(':');
                departures[i] = new Departure(Convert.ToInt32(rawdeparture[1]), Station.NameToStation(rawdeparture[0]), null, output.EndingStation, output.Route, null);
                departures[i].Station.Departures.Add(departures[i]);
            }
            output.Departures = departures;
            return output;
        }
    }
}
