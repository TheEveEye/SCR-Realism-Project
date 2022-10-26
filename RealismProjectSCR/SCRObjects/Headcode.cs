using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
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
            this.Code = Convert.ToString(Priority) + Terminus + ExtendNumber(Number);

            this.Priority = Priority;
            this.Terminus = Terminus;
            this.Number = Number;
        }
        static string ExtendNumber(int number)
        {
            if (Convert.ToString(number).Length == 1)
            {
                return "0" + number;
            }
            else
            {
                return Convert.ToString(number);
            }
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
            StationCode[] outputStationCodes = new StationCode[StationCodeCounter.TerminusStations.Length];
            for (int i = 0; i < StationCodeCounter.TerminusStations.Length; i++)
            {
                outputStationCodes[i] = new StationCode(StationCodeCounter.TerminusStations[i], StationCodeCounter.TerminusChars[i]);
            }
            StationCodeCounter output = new StationCodeCounter();
            output.StationCounters = outputStationCodes;
            return output;
        }
        public static StationCode[] GetStationHeadcodes()
        {
            StationCode[] output = new StationCode[StationCodeCounter.TerminusStations.Length];
            for (int i = 0; i < StationCodeCounter.TerminusStations.Length; i++)
            {
                output[i] = new StationCode(StationCodeCounter.TerminusStations[i], StationCodeCounter.TerminusChars[i]);
            }
            return output;
        }
    }
    public struct StationCodeCounter
    {
        public StationCode[] StationCounters { get; set; }

        public static Station[] TerminusStations = TerminusSetup();
        public static char[] TerminusChars = CharSetup();
        
        static Station[] TerminusSetup()
        {
            string[] rawStationHeadcodes = File.ReadAllLines(Program.ProjectDirectoryPath + @"SCRObjects\StationHeadcodes.txt");
            Station[] outputStations = new Station[rawStationHeadcodes.Length];
            for (int i = 0; i < rawStationHeadcodes.Length; i++)
            {
                string[] tempRaw = rawStationHeadcodes[i].Split(':');
                outputStations[i] = Station.NameToStation(tempRaw[0]);
            }
            return outputStations;
        }
        static char[] CharSetup()
        {
            string[] rawStationHeadcodes = File.ReadAllLines(Program.ProjectDirectoryPath + @"SCRObjects\StationHeadcodes.txt");
            char[] outputChars = new char[rawStationHeadcodes.Length];
            for (int i = 0; i < rawStationHeadcodes.Length; i++)
            {
                string[] tempRaw = rawStationHeadcodes[i].Split(':');
                outputChars[i] = outputChars[i] = Convert.ToChar(tempRaw[1]);
            }
            return outputChars;
        }

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
            int output = StationCounters[index].Counter;
            if (StationCounters[index].Counter != 99)
            {
                StationCounters[index].Counter++;
            }
            else
            {
                StationCounters[index].Counter = 0;
            }
            return output;
        }
    }
}