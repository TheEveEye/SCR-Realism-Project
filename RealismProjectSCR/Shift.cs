﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.Units;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;

namespace RealismProjectSCR
{
    public class Shift
    {
        public string Name { get; set; }
        public TimeFrame TimeFrame { get; set; }
        public string Description { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Leg> Legs { get; set; }
        public string Path { get; set; }

        public Shift(string Name, TimeFrame TimeFrame, string Description, List<Driver> Drivers, List<Leg> Legs)
        {
            this.Name = Name;
            this.TimeFrame = TimeFrame;
            this.Description = Description;
            this.Drivers = Drivers;
            this.Legs = Legs;

            this.Path = GetPath(this.Name); 
        }

        public void AddLeg(Leg leg)
        {
            Legs.Add(leg);
            for (int i = 0; i < leg.Departures.Length; i++)
            {
                leg.Departures[i].Station.Departures.Add(leg.Departures[i]);
            }
            Push();
        }

        public void PredictHeadcodes() // Get this done before platform allocations
        {
            Leg[] currentLegs = Leg.Sort(this.Legs, Leg.SortType.StartTime).ToArray();
            StationCodeCounter stationCodeCounter = new StationCodeCounter();
            for (int i = 0; i < currentLegs.Length; i++)
            {
                Station legTerminus = currentLegs[i].GetTerminus();
                Route legRoute = currentLegs[i].Route;

                Headcode newHeadcode = new Headcode(legRoute.HeadcodePriority, StationCodeCounter.TerminusChars[Array.IndexOf(StationCodeCounter.TerminusStations, legTerminus)], stationCodeCounter.GetAndCountUp(legTerminus));

                this.Legs[Array.IndexOf(this.Legs.ToArray(), currentLegs[i])].Headcode = newHeadcode;
            }
        }

        public void SortLegs()
        {
            int[] LegsStartingFrame = new int[this.Legs.Count];
            for (int i = 0; i < LegsStartingFrame.Length; i++)
            {
                LegsStartingFrame[i] = this.Legs[i].TimeFrame.Start;
            }

            Array.Sort(LegsStartingFrame);
        }

        public void Push()
        {
            string[] exportLegs = new string[Legs.Count];
            for (int i = 0; i < exportLegs.Length; i++)
            {
                exportLegs[i] = Leg.ToExport(Legs[i]);
            }
            File.WriteAllLines(GetPath(this.Name) + @"Legs.shift", exportLegs);

            Driver.Export(Drivers);
        }

        public static string[] NamesFromPaths(string[] paths)
        {
            string[] names = new string[paths.Length];
            for (int i = 0;i < paths.Length; i++)
            {
                names[i] = NameFromPath(paths[i]);
            }
            return names;
        }

        public static string NameFromPath(string path)
        {
            return path.Split('\\')[path.Split('\\').Length - 1];
        }

        public static string GetPath(string Name)
        {
            return Program.ProjectDirectoryPath + @"Shifts\" + Name + @"\";
        }

        public static Shift Import(string Path, string Name)
        {
            Shift output = new(Name, new TimeFrame(0, 0), "", new List<Driver>(), new List<Leg>());

            string[] shiftInfo = File.ReadAllLines(Path + @"\Info.shift");
            output.Description = shiftInfo[1];
            string[] timeFrameRaw = shiftInfo[0].Split(';');
            output.TimeFrame = new TimeFrame(Convert.ToInt32(timeFrameRaw[0]), Convert.ToInt32(timeFrameRaw[1]));
            output.Drivers = Driver.ImportAll(output.Name);
            string[] legsRaw = File.ReadAllLines(Path + @"\Legs.shift");
            Leg[] legs = new Leg[legsRaw.Length];
            for (int i = 0; i < legs.Length; i++)
            {
                legs[i] = Leg.Import(legsRaw[i], output.Drivers);
                legs[i].Driver.Legs.Add(legs[i]);
            }
            output.Legs = legs.ToList<Leg>();
            return output;
        }

        public static Shift Collect()
        {
            bool validName = false;
            bool validTime = false;
            bool validDescription = false;

            string tempInput;

            string tempShiftName = "";
            string tempStartingTime = "";
            string tempEndingTime = "";
            string tempShiftDescription = "";

            while (!validName)
            {
                Console.WriteLine("Creating New Shift... Enter Name:");
                tempInput = Console.ReadLine();
                if (String.IsNullOrEmpty(tempInput))
                {
                    Console.WriteLine("Invalid Input. Please try again...");
                }
                else
                {
                    tempShiftName = tempInput;
                    validName = true;
                }
            }
            while (!validTime)
            {
                Console.WriteLine("Enter Starting Time:");
                tempInput = Console.ReadLine();
                if ((String.IsNullOrEmpty(tempInput)) || (Time.GetTimeFormat(tempInput) != Time.Format.Time))
                {
                    Console.WriteLine("Invalid Input. Please try again...");
                }
                else
                {
                    tempStartingTime = tempInput;
                    validTime = true;
                }
            }
            validTime = false;
            while (!validTime)
            {
                Console.WriteLine("Enter Ending Time:");
                tempInput = Console.ReadLine();
                if ((String.IsNullOrEmpty(tempInput)) || (Time.GetTimeFormat(tempInput) != Time.Format.Time))
                {
                    Console.WriteLine("Invalid Input. Please try again...");
                }
                else
                {
                    tempEndingTime = tempInput;
                    validTime = true;
                }
            }
            while (!validDescription)
            {
                Console.WriteLine("Enter Description:");
                tempInput = Console.ReadLine();
                if (String.IsNullOrEmpty(tempInput))
                {
                    Console.WriteLine("Invalid Input. Please try again...");
                }
                else
                {
                    tempShiftDescription = tempInput;
                    validDescription = true;
                }
            }
            return Build(tempStartingTime, tempEndingTime, tempShiftName, tempShiftDescription);
        }

        public static Shift Create(Shift shift)
        {
            string tempShiftPath = Program.ProjectDirectoryPath + @"Shifts\" + shift.Name + @"\";
            if (Directory.Exists(tempShiftPath))
            {
                throw new IOException("Folder already exists.");
            }
            else
            {
                Directory.CreateDirectory(tempShiftPath);
                //File.Create(tempShiftPath + "Info.shift");
                //File.Create(tempShiftPath + "Legs.shift");

                string[] infoContents =
                {
                    Convert.ToString(shift.TimeFrame.Start) + ";" + Convert.ToString(shift.TimeFrame.End),
                    shift.Description
                };
                File.WriteAllLines(tempShiftPath + "Info.shift", infoContents);
                File.WriteAllLines(tempShiftPath + "Legs.shift", new string[0]);
                File.WriteAllLines(tempShiftPath + "Drivers.shift", new string[0]);
                return shift;
            }
        }

        public static Shift Build(int startingFrame, int endingFrame, string shiftName, string description)
        {
            return new Shift(shiftName, new TimeFrame(startingFrame, endingFrame), description, new List<Driver>(), new List<Leg>());
        }

        public static Shift Build(string startingTime, string endingTime, string shiftName, string description)
        {
            return new Shift(shiftName, new TimeFrame(Time.TimeToSeconds(startingTime) / 15, Time.TimeToSeconds(endingTime) / 15), description, new List<Driver>(), new List<Leg>());
        }

        public string[] LegsToDebug()
        {
            List<Leg> sortedLegs = Leg.Sort(Legs, Leg.SortType.StartTime);
            List<string> output = new List<string>();
            for (int i = 0; i < Legs.Count; i++)
            {
                output.Add(sortedLegs[i].ToCompact(i + 1));
            }
            return output.ToArray();
        }
    }
}
