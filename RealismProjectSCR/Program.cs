using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;
using RealismProjectSCR.NetworkPlanner;
using RealismProjectSCR;

class Program
{
    public static string StartupFilePath;
    public static string[] StartupFile;
    public static Station[] Stations;
    public static Route[] Routes;
    public static string[] StationNames;
    public static string ProjectDirectoryPath;
    public static Timing[] DepotTimings;
    public static List<string> ShiftPaths;
    public static List<string> ShiftNames;
    public static Shift ActiveShift;

    static void Main()
    {
        Console.Title = "Realism Project Network Planner Build 29";
        Console.WriteLine("Importing Program Data...");

        string rawProjectDirectoryPath = Path.GetFullPath(@"RealismProjectSCR.startup"); // This doesn't work yet
        string[] splittedProjectDirectoryPath = rawProjectDirectoryPath.Split('\\');
        string projectDirectoryPath = "";
        for (int i = 0; i < splittedProjectDirectoryPath.Length - 4; i++)
        {
            projectDirectoryPath += splittedProjectDirectoryPath[i] + @"\";
        }
        ProjectDirectoryPath = projectDirectoryPath;
        StartupFilePath = projectDirectoryPath + "RealismProjectSCR.startup";
        StartupFile = File.ReadAllLines(StartupFilePath);
        Station[] _Stations = new Station[Convert.ToInt32(StartupFile[0])]; // startupFile[0] = amount of stations
        StationNames = StartupFile[1].Split(';'); // startupFile[0] = station names



        Console.WriteLine("Importing Station Data...");
        string[] AdjacentStations = File.ReadAllLines(ProjectDirectoryPath + @"SCRObjects\AdjacentStations.txt");
        for (int i = 0; i < _Stations.Length; i++) // This for-loop 
        {
            _Stations[i] = new Station(StationNames[i], null, null, new List<Departure>()); // Fills in the name of the station
            _Stations[i].GetSetShortcuts();
            // string[] StationInfo = File.ReadAllLines(_Stations[i].stblPath); // Gets all information from the .stbl station file   
        }
        Stations = _Stations;
        for (int i = 0; i < Stations.Length; i++)
        {
            Stations[i].AdjacentStations = Station.NamesToStations(AdjacentStations[i].Split(':')[1].Split(';'));
        }


        // Temporary code to automatically enter adjacent stations
        /*
        string[] AdjacentStations = File.ReadAllLines(StartupFilePath + @"SCRObjects\AdjacentStations.txt");
        foreach (var station in AdjacentStations)
        {
            string[] temp = station.Split(':');
            Console.WriteLine("Importing Adjacent Station for: " + temp[0]);
            File.WriteAllLines(Station.NameToStation(temp[0]).stblPath, new string[] { temp[1] });
        }
        */

        Console.WriteLine("Importing Route Data...");
        Routes = Route.Import();

        Console.WriteLine("Importing Shift Data...");
        ShiftPaths = GetShiftPaths();
        ShiftNames = Shift.NamesFromPaths(ShiftPaths.ToArray()).ToList<string>();

        Console.WriteLine("----------------------------------------------------------------");
        Console.WriteLine("SCR Realism Project v1.10.0 Build 29");
        Console.WriteLine("Developed by Eve");
        Console.WriteLine("Enter \"help\" or \"commands\" to get a list of commands.");
        Console.WriteLine("----------------------------------------------------------------");

        bool selectedShift = false;
        while (!selectedShift)
        {
            if (ShiftNames.Count == 0)
            {
                ActiveShift = Shift.Create(Shift.Collect());
                ShiftNames.Add(ActiveShift.Name);
                ShiftPaths.Add(ActiveShift.Path);
            }
            else
            {
                Tab();
                Console.WriteLine("Select Shift:");
                for (int i = 0; i < ShiftNames.Count; i++)
                {
                    Console.WriteLine((i + 1) + " - " + ShiftNames[i]);
                }
                Console.WriteLine((ShiftNames.Count + 1) + " - Create New");

                string selectedShiftInput = Console.ReadLine();
                int selectedShiftIndex = ShiftNames.Count + 2;
                //try
                //{
                    selectedShiftIndex = Convert.ToInt32(selectedShiftInput);
                    if (selectedShiftIndex == ShiftNames.Count + 1)
                    {
                        ActiveShift = Shift.Create(Shift.Collect());
                        ShiftNames.Add(ActiveShift.Name);
                        ShiftPaths.Add(ActiveShift.Path);
                    }
                    else
                    {
                        ActiveShift = Shift.Import(ShiftPaths[selectedShiftIndex - 1]);
                        selectedShift = true;
                    }
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Invalid Argument given. Please try again...");
                //}
            }
        }

        Console.WriteLine("Waiting for input...");

        /*
        // When debugging station data, use this
        Station TestStation = new("Beechly", null, null, null);
        Console.WriteLine(TestStation.stblPath);
        */

        bool proceed = false; // Use for all menu checks
        bool close = false;
        string EnteredCommandRaw = null;

        while (!close)
        {
            Tab();
            while (!proceed)
            {
                EnteredCommandRaw = Console.ReadLine().ToLower();

                if (EnteredCommandRaw != null) // Check if entered command isn't null
                {
                    proceed = true;
                }
                else
                {
                    Console.WriteLine("Invalid Command. Please try again...");
                }
            }
            proceed = false;

            string[] EnteredCommand = EnteredCommandRaw.Split(' '); // Splitting up entered command into each arguement

            switch (EnteredCommand[0])
            {
                case "helloworld": // Test Command to see if it works
                    Console.WriteLine("Hello World!");
                    break;

                case "close": // Closes the Program
                    close = true;
                    break;

                case "help":
                case "commands":

                    break;

                case "add":
                case "new":
                case "create":
                    if (EnteredCommand.Length < 2)
                    {
                        Console.WriteLine("Not enough Arguments given. Please try again...");
                        break;
                    }
                    switch (EnteredCommand[1])
                    {
                        case "leg":
                            if (EnteredCommand.Length < 6)
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            if (ActiveShift == null)
                            {
                                Console.WriteLine("Please select a shift first...");
                                break;
                            }
                            try
                            {
                                Convert.ToInt32(EnteredCommand[2]);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            Route route = Routes[Convert.ToInt32(EnteredCommand[2]) - 1];
                            try
                            {
                                Convert.ToInt32(EnteredCommand[3]);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            int StartingFrame = Convert.ToInt32(EnteredCommand[3]);
                            Station startingStation = Station.FromArgument(EnteredCommand[4]);
                            Station endingStation = Station.FromArgument(EnteredCommand[5]);
                            if ((startingStation == null) || (endingStation == null)) // Find a way to get station names that are 2 or more words like "City Hospital" into one argument
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            Leg createdLeg = Leg.Create(route, StartingFrame + ActiveShift.TimeFrame.Start, startingStation, endingStation);
                            ActiveShift.AddLeg(createdLeg);
                            break;

                        case "shift":

                            Shift tempShift = Shift.Create(Shift.Collect());
                            ShiftNames.Add(ActiveShift.Name);
                            ShiftPaths.Add(ActiveShift.Path);
                            Console.WriteLine("Do you want to set it as your Active Shift?");
                            if (BoolFromInput(Console.ReadLine()) == true)
                            {
                                Console.WriteLine("Okay, Selected Shift: " + tempShift.Name);
                                ActiveShift = tempShift;
                            }
                            else
                            {
                                Console.WriteLine("Okay, Selected Shift: " + ActiveShift.Name);
                            }
                            break;

                        default: // Not an existing command
                            Console.WriteLine("Invalid Argument given. Please try again...");
                            break;
                    }
                    break;

                case "get": // Allows you to get any information
                    if (EnteredCommand.Length < 2)
                    {
                        Console.WriteLine("Not enough Arguments given. Please try again...");
                        break;
                    }
                    switch (EnteredCommand[1]) // Looks at the second argument given
                    {
                        case "elapsedseconds": // Gets the amount of seconds that have passed today
                            Console.WriteLine(Time.ElapsedSeconds());
                            break;

                        case "elapsedframes":
                        case "elapsedscheduleframes":
                            Console.WriteLine(Time.ElapsedFrames()); // Gets the amount of schedule frames that have passed today
                            break;

                        case "stationtable":
                        case "stationschedule":
                            Station searchedStation = Station.FromArgument(EnteredCommand[2]);
                            if (searchedStation == null) // Find a way to get station names that are 2 or more words like "City Hospital" into one argument
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            searchedStation.SortDepartures();
                            if (EnteredCommand.Length < 4)
                            {
                                searchedStation.PrintDepartures();
                                break;
                            }
                            if (EnteredCommand[3] == "all")
                            {
                                searchedStation.PrintDepartures();
                                break;
                            }
                            if ((EnteredCommand[3] != "all") && (EnteredCommand.Length < 5))
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            if (EnteredCommand.Length >= 5)
                            {
                                try
                                {
                                    Convert.ToInt32(EnteredCommand[3]);
                                    Convert.ToInt32(EnteredCommand[4]);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("Invalid Argument given. Please try again...");
                                    break;
                                }
                                searchedStation.PrintDepartures(Convert.ToInt32(EnteredCommand[3]), Convert.ToInt32(EnteredCommand[4]));
                                break;
                            }

                            break;

                        case "route":
                        case "routetimings":
                        case "routedata":
                            if (EnteredCommand.Length < 3)
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            try
                            {
                                Convert.ToInt32(EnteredCommand[2]);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            Route.PrintData(Convert.ToInt32(EnteredCommand[2]));
                            break;

                        case "leg":
                        case "legtimings":
                        case "legdata":
                            if (EnteredCommand.Length < 3)
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            try
                            {
                                Convert.ToInt32(EnteredCommand[2]);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            if (ActiveShift.Legs.Count < Convert.ToInt32(EnteredCommand[2]))
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            string[] legData = ActiveShift.Legs[Convert.ToInt32(EnteredCommand[2]) - 1].ToDriver();
                            foreach (string item in legData)
                            {
                                Console.WriteLine(item);
                            }

                            break;

                        case "legs":
                        case "leglist":
                            string[] compactLegs = ActiveShift.LegsToDebug();
                            Console.WriteLine("----------------------------------------------------------------");
                            foreach (string item in compactLegs)
                            {
                                Console.WriteLine(item);
                            }
                            Console.WriteLine("----------------------------------------------------------------");
                            break;

                        default: // Not an existing command
                            Console.WriteLine("Invalid Argument given. Please try again...");
                            break;
                    }
                    break;

                case "calculate": // Calculates specific values using functions created in other classes
                case "calc":
                    if (EnteredCommand.Length < 4) // Minimum of 4 arguments because all functions inside this command need at least 4.
                    {
                        Console.WriteLine("Not enough Arguments given. Please try again...");
                        break;
                    }

                    switch (EnteredCommand[1]) // Looks at the second argument given
                    {
                        case "seconds":
                            if (Time.GetTimeFormat(EnteredCommand[3]) != Time.FormatSeconds)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            switch (EnteredCommand[2])
                            {
                                case "frames":
                                case "scheduleframes":
                                    Console.WriteLine(Convert.ToInt32(EnteredCommand[3]) / 15);
                                    break;

                                case "time":
                                    Console.WriteLine(Time.SecondsToDateTime(Convert.ToInt32(EnteredCommand[3])).ToLongTimeString());
                                    break;

                                default: // Not an existing command
                                    Console.WriteLine("Invalid Argument given. Please try again...");
                                    break;
                            }
                            break;

                        case "frames":
                        case "scheduleframes":
                            if (Time.GetTimeFormat(EnteredCommand[3]) != Time.FormatFrames)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            switch (EnteredCommand[2])
                            {
                                case "seconds":
                                    Console.WriteLine(Convert.ToInt32(EnteredCommand[3]) * 15);
                                    break;

                                case "time":
                                    Console.WriteLine(Time.SecondsToDateTime(Convert.ToInt32(EnteredCommand[3]) * 15).ToLongTimeString());
                                    break;

                                default: // Not an existing command
                                    Console.WriteLine("Invalid Argument given. Please try again...");
                                    break;
                            }

                            break;

                        case "time":
                            if (Time.GetTimeFormat(EnteredCommand[3]) != Time.FormatTime)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            switch (EnteredCommand[2])
                            {
                                case "seconds":
                                    Console.WriteLine(Time.TimeToSeconds(EnteredCommand[3]));
                                    break;

                                case "frames":
                                case "scheduleframes":
                                    Console.WriteLine(Time.TimeToSeconds(EnteredCommand[3]) / 15);
                                    break;

                                default: // Not an existing command
                                    Console.WriteLine("Invalid Argument given. Please try again...");
                                    break;
                            }

                            break;

                        default: // Not an existing command
                            Console.WriteLine("Invalid Argument given. Please try again...");
                            break;
                    }
                    break;

                case "":

                    break;

                default: // Not an existing command
                    Console.WriteLine("Invalid Command. Please try again...");
                    break;
            }
        }
    }
    static List<string> GetShiftPaths()
    {
        string path = ProjectDirectoryPath + @"Shifts\";
        return Directory.GetDirectories(ProjectDirectoryPath + @"Shifts\").ToList<string>();
    }
    public static int[] IndexesOf(int obj, int[] array)
    {
        List<int> output = new List<int>();
        for (int i = 0; i < array.Length; i++)
        {
            if (obj == array[i])
            {
                output.Add(i);
            }
        }
        return output.ToArray();
    }
    public static string BuildString(string[] strings, string seperator)
    {
        if (strings.Length < 1)
        {
            return string.Empty;
        }
        string output = strings[0];
        for (int i = 1; i < strings.Length; i++)
        {
            output += seperator + strings[i];
        }
        return output;
    }
    public static bool Contains(string obj1, string[] array)
    {
        bool output = false;
        foreach (var item in array)
        {
            if (item.ToLower() == obj1.ToLower())
            {
                output = true;
            }
        }
        return output;
    }
    public static void Tab()
    {
        Console.WriteLine("");
    }
    public static string IntToSpaces(int amount)
    {
        string output = "";
        for (int i = 0; i < amount; i++)
        {
            output += " ";
        }
        return output;
    }
    public static string FillWithSpaces(string current, int length)
    {
        return current + IntToSpaces(length - current.Length);
    }
    public static bool? BoolFromInput(string input)
    {
        bool? output = null;
        string[] trueList = new string[]
        { 
            "yes",
            "true",
            "y",
            "positive"
        };
        string[] falseList = new string[]
        {
            "no",
            "false",
            "n",
            "negative"
        };
        if (String.IsNullOrEmpty(input))
        {
            output = null;
        }
        else if (trueList.Contains(input))
        {
            output = true;
        }
        else if (falseList.Contains(input))
        {
            output = false;
        }
        return output;
    }
}
