using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.NetworkPlanner;
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

        public Leg(Route Route, int StartFrame, int EndFrame, Departure[] Departures, Station StartingStation, Station EndingStation, Headcode? Headcode) //Constructor with startFrame, endFrame and Headcode
        {
            this.Route = Route;
            this.TimeFrame = new(StartFrame, EndFrame);
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
            this.Headcode = Headcode;
        }
        public Leg(Route Route, TimeFrame TimeFrame, Departure[] Departures, Station StartingStation, Station EndingStation, Headcode? Headcode) //Constructor with TimeFrame and Headcode
        {
            this.Route = Route;
            this.TimeFrame = TimeFrame;
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
            this.Headcode = Headcode;
        }
        public Leg(Route Route, int StartFrame, int EndFrame, Departure[] Departures, Station StartingStation, Station EndingStation) //Constructor with startFrame and endFrame
        {
            this.Route = Route;
            this.TimeFrame = new(StartFrame, EndFrame);
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
        }
        public Leg(Route Route, TimeFrame TimeFrame, Departure[] Departures, Station StartingStation, Station EndingStation) //Constructor with TimeFrame
        {
            this.Route = Route;
            this.TimeFrame = TimeFrame;
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
        }
        public static Leg Create(Route route, int startingFrame, Station startingStation, Station endingStation)
        {
            Leg output = new Leg(route, new(0, 0), null, startingStation, endingStation, null);
            output.Departures = Route.GetLegDepartures(route, startingFrame, startingStation, endingStation);
            output.TimeFrame = DeparturesTimeFrame(output.Departures);

            return output;
        }
        public static TimeFrame DeparturesTimeFrame(Departure[] departures)
        {
            return new TimeFrame(departures[0].Frame, departures[departures.Length - 1].Frame);
        }
        public string ToDriver()
        {
            List<string> output = new List<string>();

            output.Add("----------------------------------------------------------------");

            output.Add("Departures for Leg " + (Array.IndexOf(Program.ActiveShift.Legs.ToArray(), this) + 1));
            output.Add("Starting Time: " + TimeFrame.StartTime + "    Route: " + Route.RouteNumberString(Route.RouteNumber) + " (" + Route.Name + ")    Last Station: " + Departures[Departures.Length - 1].Station.Name);
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

            output.Add("----------------------------------------------------------------");
            
            string outputString = "";
            foreach (string Line in output) // Building String
            {
                outputString += Line;
            }
            
            return outputString;
        }
        public static string ToExport(Leg leg)
        {
            string output = Convert.ToString(leg.Route.RouteNumber) + ";" + Convert.ToString(leg.TimeFrame.Start) + "," + Convert.ToString(leg.TimeFrame.End) + ";" + leg.StartingStation.Name + "," + leg.EndingStation.Name + ";" + leg.Departures[0].Station.Name + ":" + Convert.ToString(leg.Departures[0].Frame);
            for (int i = 1; i < leg.Departures.Length; i++)
            {
                output += "," + leg.Departures[i].Station.Name + ":" + Convert.ToString(leg.Departures[i].Frame);
            }
            return output;
        }
        public string ToCompact(int LegNumber)
        {
            return "Leg " + LegNumber + "    Route: " + Route.RouteNumberString(this.Route.RouteNumber) + " (" + this.Route.Name + ")    Time Frame: " + this.TimeFrame.StartTime + " - " + this.TimeFrame.EndTime + " (" + this.TimeFrame.Start + " - " + this.TimeFrame.End + ")    Driving: " + this.StartingStation.Name + " - " + this.EndingStation.Name;
        }
        public static Leg Import(string input)
        {
            string[] split = input.Split(';');
            Leg output = new Leg(null, 0, 0, new Departure[0], null, null, null);
            output.Route = Program.Routes[Convert.ToInt32(split[0]) - 1];
            string[] timeframe = split[1].Split(',');
            output.TimeFrame = new(Convert.ToInt32(timeframe[0]), Convert.ToInt32(timeframe[1]));
            output.TotalFrames = output.TimeFrame.ToFrames();
            string[] terminus = split[2].Split(',');
            output.StartingStation = Station.NameToStation(terminus[0]);
            output.EndingStation = Station.NameToStation(terminus[1]);

            string[] rawdepartures = split[3].Split(',');
            Departure[] departures = new Departure[rawdepartures.Length];
            for (int i = 0; i < departures.Length; i++)
            {
                string[] rawdeparture = rawdepartures[i].Split(':');
                departures[i] = new Departure(Convert.ToInt32(rawdeparture[1]), Station.NameToStation(rawdeparture[0]), null, output.EndingStation, output.Route);
                departures[i].Station.Departures.Add(departures[i]);
            }
            output.Departures = departures;
            return output;
        }
    }
}
