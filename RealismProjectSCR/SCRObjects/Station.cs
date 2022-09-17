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
            

            for (int i = 0; i < Program.Stations.Length; i++)
            {
                if (Program.Stations[i].Name.ToLower() == Name.ToLower())
                {
                    return Program.Stations[i];
                }
            }
            return null;
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

        }

        public void PrintDepartures()
        {
            string[] departures = GetDepartures();
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
        
        public string[] GetDepartures()
        {
            if (this.Departures == null)
            {
                return null;
            }
            else
            {
                string[] output = new string[this.Departures.Count];
                for (int i = 0; i < Departures.Count; i++)
                {
                    output[i] = this.Departures[i].ToDebug();
                }
                return output;
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
    }
}