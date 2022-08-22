using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects.TimeTables;

namespace RealismProjectSCR.SCRObjects
{
    public class Station
    {
        public string Name { get; set; }
        public Station[] AdjacentStations { get; set; }
        public Platform[] Platforms { get; set; }
        public Departure[] Departures { get; set; }

        public string stblPath { get; }

        public Station(string Name, Station[] AdjacentStations, Platform[] Platforms, Departure[] Departures)
        {
            this.Name = Name;
            this.AdjacentStations = AdjacentStations;
            this.Platforms = Platforms;
            this.Departures = Departures;
            this.stblPath = @"\RealismProjectSCR\RealismProjectSCR\SCRObjects\TimeTables\StationTables\" + this.Name + ".stbl"; // This doesn't work. Research on Path.GetFullPath()
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
                if (Program.Stations[i].Name == Name)
                {
                    return Program.Stations[i];
                }
                
            }
            return null;
        }
    }
}