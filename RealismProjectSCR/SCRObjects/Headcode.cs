using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.NetworkPlanner;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;

namespace RealismProjectSCR.SCRObjects
{
    public class Headcode
    {
        public string Code { get; set; }
        public int Priority { get; set; }
        public char Terminus { get; set; }
        public int Number { get; set; }

        public static StationCode[] StationHeadcodes = StationCode.GetStationHeadcodes();

        public Headcode(int Priority, char Terminus, int Number)
        {
            this.Code = Convert.ToString(Priority) + Terminus + Convert.ToString(Number);

            this.Priority = Priority;
            this.Terminus = Terminus;
            this.Number = Number;
        }
    }
    public struct StationCode
    {
        public Station Station { get; set; }
        public char Character { get; set; }
        public int Counter { get; set; }

        public StationCode(Station Station, char Character)
        {
            this.Station = Station;
            this.Character = Character;
            this.Counter = 0;
        }

        public static StationCodeCounter GetStationCodeCounter()
        {
            string[] rawStationHeadcodes = File.ReadAllLines(Program.ProjectDirectoryPath + @"SCRObjects\StationHeadcodes.txt");

            char[] outputChars = new char[rawStationHeadcodes.Length];
            Station[] outputStations = new Station[rawStationHeadcodes.Length];

            for (int i = 0; i < rawStationHeadcodes.Length; i++)
            {
                string[] tempRaw = rawStationHeadcodes[i].Split(':');
                outputStations[i] = Station.NameToStation(tempRaw[0]);
                outputChars[i] = Convert.ToChar(tempRaw[1]);
            }
            StationCode[] outputStationCodes = new StationCode[rawStationHeadcodes.Length];
            for (int i = 0; i < rawStationHeadcodes.Length; i++)
            {
                outputStationCodes[i] = new StationCode(outputStations[i], outputChars[i]);
            }
            StationCodeCounter output = new StationCodeCounter();
            StationCodeCounter.TerminusStations = outputStations;
            StationCodeCounter.TerminusChars = outputChars;
            output.StationCounters = outputStationCodes;
            return output;
        }
        public static StationCode[] GetStationHeadcodes()
        {
            return GetStationCodeCounter().StationCounters;
        }
    }
    public struct StationCodeCounter
    {
        public StationCode[] StationCounters { get; set; }
        public static Station[] TerminusStations { get; set; }
        public static char[] TerminusChars { get; set; }

        public StationCodeCounter()
        {
            StationCode[] tempStationCounters = StationCode.GetStationHeadcodes();
            for (int i = 0; i < tempStationCounters.Length; i++)
            {
                tempStationCounters[i].Counter = 0;
            }
            this.StationCounters = tempStationCounters;
        }
        public int GetAndCountUp(Station Terminus)
        {
            int index = Array.IndexOf(TerminusStations, Terminus);
            StationCounters[index].Counter++;
            return StationCounters[index].Counter;
        }
    }
}