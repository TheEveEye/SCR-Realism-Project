using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;

namespace RealismProjectSCR.SCRObjects
{
    public class Station
    {
        public string Name { get; set; }
        public Station[] AdjacentStations { get; set; }
        public Platform[] Platforms { get; set; }
        public List<Departure> Departures { get; set; }

        public string stblPath { get; }

        string[] Shortcuts { get; set; }
        static string[] ShortcutsRaw = File.ReadAllLines(Program.ProjectDirectoryPath + @"SCRObjects\StationShortcuts.txt");

        public Station(string Name, Station[] AdjacentStations, Platform[] Platforms, List<Departure> Departures)
        {
            this.Name = Name;
            this.AdjacentStations = AdjacentStations;
            this.Platforms = Platforms;
            this.Departures = Departures;
            this.stblPath = Program.ProjectDirectoryPath + @"SCRObjects\TimeTables\StationTables\" + this.Name + ".stbl";
        }  

        public static Station[] NamesToStations(string[] Names)
        {
            Station[] Stations = new Station[Names.Length];
            for (int i = 0; i < Names.Length; i++)
            {
                Stations[i] = NameToStation(Names[i]);
            }
            return Stations;
        }
        public static Station NameToStation(string Name)
        {
            if (Program.StationNames.Contains(Name))
            {
                return Program.Stations[Array.IndexOf(Program.StationNames, Name)];
            }
            else
            {
                return null;
            }
        }
        public static Station FromArgument(string Argument)
        {
            if (Argument.Split('-').Length == 1)
            {
                if (Program.Contains(Argument, Program.StationNames))
                {
                    return NameToStation(Argument);
                }
                else
                {
                    for (int i = 0; i < Program.Stations.Length; i++)
                    {
                        if (Program.Contains(Argument, Program.Stations[i].Shortcuts))
                        {
                            return Program.Stations[i];
                        }
                    }
                }
            }
            else if (Argument.Split('-').Length > 1)
            {
                string BuiltName = Program.BuildString(Argument.Split('-'), " ");
                if (Program.Contains(BuiltName, Program.StationNames))
                {
                    return NameToStation(BuiltName);
                }
            }
            return null;
        }

        public void SortDepartures()
        {
            int[] rawDepartureFrames = new int[this.Departures.Count];
            for (int i = 0; i < rawDepartureFrames.Length; i++)
            {
                rawDepartureFrames[i] = this.Departures[i].Frame;
            }
            int[] SortedDepartureFrames = new int[rawDepartureFrames.Length];
            rawDepartureFrames.CopyTo(SortedDepartureFrames, 0);
            Array.Sort(SortedDepartureFrames);
            int SortStart = SortedDepartureFrames[0];
            int SortEnd = SortedDepartureFrames[SortedDepartureFrames.Length - 1];

            List<Departure> SortedDepartures = new List<Departure>();
            for (int i = SortStart; i < SortEnd; i++)
            {
                int[] tempIndexes = Program.IndexesOf(i, rawDepartureFrames.ToArray<int>());
                for (int j = 0; j < tempIndexes.Length; j++)
                {
                    SortedDepartures.Add(this.Departures[tempIndexes[j]]);
                }
            }
            this.Departures = SortedDepartures;
        }

        public void PrintDepartures()
        {
            string[] departures = GetDepartures(false);
            Console.WriteLine("Departures for: " + this.Name);
            if ((departures == null) || (departures.Length == 0))
            {
                Console.WriteLine("There are currently no scheduled trains for this station.");
            }
            else
            {
                foreach (string departure in departures)
                {
                    Console.WriteLine(departure);
                }
            }
        }
        public void PrintDepartures(int timeStart, int timeEnd)
        {
            string[] departures = GetDepartures(new TimeFrame(timeStart, timeEnd));
            Console.WriteLine("Departures for: " + this.Name);
            if ((departures == null) || (departures.Length == 0))
            {
                Console.WriteLine("There are currently no scheduled trains for this station.");
            }
            else
            {
                foreach (string departure in departures)
                {
                    if (!String.IsNullOrEmpty(departure))
                    {
                        Console.WriteLine(departure);
                    }
                }
            }
        }

        public string[] GetDepartures(bool showArrivals)
        {
            if (this.Departures == null)
            {
                return null;
            }
            else
            {
                List<string> output = new List<string>();
                for (int i = 0; i < Departures.Count; i++)
                {
                    if ((this.Departures[i].Station == this.Departures[i].Terminus) && (!showArrivals))
                    {
                        
                    }
                    else
                    {
                        output.Add(this.Departures[i].ToPassenger());
                    }
                }
                return output.ToArray();
            }
        }
        public string[] GetDepartures(TimeFrame TimeFrame)
        {
            string[] output = new string[this.Departures.Count];
            for (int i = 0; i < Departures.Count; i++)
            {
                if ((Departures[i].Frame > TimeFrame.Start) && (Departures[i].Frame < TimeFrame.End))
                {
                    output[i] = this.Departures[i].ToDebug();
                }
            }
            return output;
        }
        public void GetSetShortcuts()
        {
            for (int i = 0; i < ShortcutsRaw.Length; i++)
            {
                if (ShortcutsRaw[i].Split(':')[0] == this.Name)
                {
                    this.Shortcuts = ShortcutsRaw[i].Split(':')[1].Split(';');
                }
            }
        }
        public static int FindLongestTerminusStationNameLength(Departure[] departures)
        {
            int longest = 0;
            foreach (var terminus in departures)
            {
                int tempLength = terminus.Terminus.Name.Length;
                if (tempLength > longest)
                {
                    longest = tempLength;
                }
            }
            return longest;
        }
        public static int FindLongestRouteNameLength(Departure[] departures)
        {
            int longest = 0;
            foreach (var route in departures)
            {
                string tempString = String.Format("{0} ({1})", Route.RouteNumberString(route.Route.RouteNumber), route.Route.Name);
                int tempLength = tempString.Length;
                if (tempLength > longest)
                {
                    longest = tempLength;
                }
            }
            return longest;
        }
        public string ToStationTable()
        {
            int LongestTerminusStationNameLength = FindLongestTerminusStationNameLength(this.Departures.ToArray());
            int LongestRouteNameLength = FindLongestRouteNameLength(this.Departures.ToArray());

            List<string> output = new List<string>();
            output.Add(String.Format("Departure for {0}    Total Departures: {1}", this.Name, this.Departures.Count));
            if (this.Departures.Count == 0)
            {
                output.Add("This station currently has no scheduled departures.");
            }
            else
            {
                output.Add("Time:       " + Program.FillWithSpaces("Route:", LongestRouteNameLength + 4) + Program.FillWithSpaces("Terminus:", LongestTerminusStationNameLength + 4));
            }
            foreach (Departure departure in this.Departures)
            {
                output.Add(String.Format("{0}    {1}", Time.ScheduleFramesToDateTime(departure.Frame).ToLongTimeString(), departure.Route.ToNumberNameOutput()));
            }


            string outputString = "";
            foreach (string line in output)
            {
                outputString += line + "\n";
            }
            return outputString;
        }
    }
}