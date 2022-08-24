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
        public Departure[] Departures { get; set; }

        public string stblPath { get; }

        public Station(string Name, Station[] AdjacentStations, Platform[] Platforms, Departure[] Departures)
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
        public void PrintDepartures()
        {
            string[] departures = GetDepartures();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Departures for: " + this.Name);
            Console.WriteLine("--------------------------------");
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
                string[] output = new string[this.Departures.Length];
                for (int i = 0; i < Departures.Length; i++)
                {
                    output[i] = this.Departures[i].ToDebug();
                }
                return output;
            }
        }
        public string[] GetDepartures(TimeFrame TimeFrame)
        {
            string[] output = new string[this.Departures.Length];
            for (int i = 0; i < Departures.Length; i++)
            {
                if ((Departures[i].Frame > TimeFrame.Start) && (Departures[i].Frame < TimeFrame.End))
                {
                    output[i] = this.Departures[i].ToDebug();
                }
            }
            return output;
        }
    }
}