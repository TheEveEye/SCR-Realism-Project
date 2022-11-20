using RealismProjectSCR.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class Driver
    {
        public Route Route { get; set; }
        public int SpawningFrame { get; set; }
        public List<Leg> Legs { get; set; }
        public string PlayerName { get; set; }

        public Driver(Route Route, int SpawningFrame, List<Leg> Legs, string PlayerName)
        {
            this.Route = Route;
            this.SpawningFrame = SpawningFrame;
            this.Legs = Legs;
            this.PlayerName = PlayerName;
        }

        public static string[] ToCompacts(Driver[] drivers)
        {
            string[] output = new string[drivers.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = String.Format("{0}: {1}   Route: {2}    {3} Legs    Spawn Time: {4}", i, drivers[i].PlayerName, drivers[i].Route.ToNumberNameOutput(), drivers[i].Legs.Count, Time.ScheduleFramesToDateTime(drivers[i].SpawningFrame).ToLongTimeString());
            }
            return output;
        }

        public string ToExport()
        {
            // cvlai:1:4560
            return String.Format("{0}:{1}:{2}", PlayerName, Route.RouteNumber, SpawningFrame);
        }
        public static void Export(List<Driver> drivers)
        {
            string[] exportedDrivers = new string[drivers.Count];
            for (int i = 0; i < exportedDrivers.Length; i++)
            {
                exportedDrivers[i] = drivers[i].ToExport();
            }
            File.WriteAllLines(Program.ProjectDirectoryPath + @"Shifts\" + Program.ActiveShift.Name + @"\Drivers.shift", exportedDrivers);
        }
        public static Driver Import(string input)
        {
            try
            {
                string[] parameters = input.Split(':');
                Driver output = new Driver(Program.Routes[Convert.ToInt32(parameters[1]) - 1], Convert.ToInt32(parameters[2]), new List<Leg>(), parameters[0]);
                return output;
            }
            catch (Exception)
            {
                throw new Exception("Fatal error whilst importing Drivers.");
            }
        }
        public static List<Driver> ImportAll(string activeShiftName)
        {
            List<Driver> drivers = new List<Driver>();
            string[] importedDrivers = File.ReadAllLines(Program.ProjectDirectoryPath + @"Shifts\" + activeShiftName + @"\Drivers.shift");
            foreach (string driver in importedDrivers)
            {
                drivers.Add(Import(driver));
            }
            return drivers;
        }

        public static Driver GetDriverInteractive(bool cancelOption = false)
        {
            Console.WriteLine(Program.BuildString(ToCompacts(Program.ActiveShift.Drivers.ToArray()), "\n"));
            Console.WriteLine("Choose a driver index...");
            int chosenIndex = -1;
            string inputRaw = "";
            bool validInput = false;
            Driver output = null;
            while (!validInput)
            {
                inputRaw = Console.ReadLine();
                if ((inputRaw.ToLower() == "cancel") && cancelOption)
                {
                    throw new Exception("cancelled");
                }
                try
                {
                    chosenIndex = Convert.ToInt32(inputRaw);
                    output = Program.ActiveShift.Drivers[chosenIndex];
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid index, please enter a valid driver index...");
                }
            }
            return output;
        }

        public static TimeFrame GetTimeFrameInteractive(Driver driver, bool cancelOption = false)
        {
            throw new NotImplementedException();
        }

        public void SortLegs(Leg.SortType sortType)
        {
            this.Legs = Leg.Sort(this.Legs, sortType);
        }
    }
}
