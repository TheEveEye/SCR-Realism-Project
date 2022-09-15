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

        public Leg(Route Route, int StartFrame, int EndFrame, Departure[] Departures, Station StartingStation, Station EndingStation)
        {
            this.Route = Route;
            this.TimeFrame = new(StartFrame, EndFrame);
            this.TotalFrames = this.TimeFrame.ToFrames();
            this.Departures = Departures;
            this.StartingStation = StartingStation;
            this.EndingStation = EndingStation;
        }
        public Leg(Route Route, TimeFrame TimeFrame, Departure[] Departures, Station StartingStation, Station EndingStation)
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
            Leg output = new Leg(route, new(0, 0), null, startingStation, endingStation);
            output.Departures = Route.GetLegDepartures(route, startingFrame, startingStation, endingStation);
            output.TimeFrame = DeparturesTimeFrame(output.Departures);

            return output;
        }
        public static TimeFrame DeparturesTimeFrame(Departure[] departures)
        {
            TimeFrame output = new TimeFrame(departures[0].Frame, 0);
            int timer = output.Start;
            for (int i = 0; i < departures.Length; i++)
            {
                timer += departures[i].Frame;
            }
            output.End = timer;
            return output;
        }
        public string[] ToDriver()
        {
            string[] output = new string[2 + Departures.Length];

            return output;
        }
        public string ToExport()
        {
            string output = Convert.ToString(Route.RouteNumber) + ";" + Convert.ToString(TimeFrame.Start) + "," + Convert.ToString(TimeFrame.End) + ";" + StartingStation.Name + "," + EndingStation.Name + ";" + Departures[0].Station.Name + ":" + Convert.ToString(Departures[0].Frame);
            for (int i = 1; i < Departures.Length; i++)
            {
                output += "," + Departures[i].Station.Name + ":" + Convert.ToString(Departures[i].Frame);
            }
            return output;
        }
        public static Leg Import(string input)
        {
            string[] split = input.Split(';');
            Leg output = new Leg(null, 0, 0, null, null, null);
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
            }
            return output;
        }
    }
}
