using System;
using System.Collections.Generic;
using System.Collections;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;
using RealismProjectSCR.NetworkPlanner;

class Program
{
    public static string StartupFilePath;
    public static string[] StartupFile;
    public static Station[] Stations;
    public static Route[] Routes;
    public static string[] StationNames;
    public static string ProjectDirectoryPath;

    static void Main()
    {
        Console.Title = "Realism Project Network Planner v0.1 Beta";
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
        for (int i = 0; i < _Stations.Length; i++) // This for-loop 
        {
            _Stations[i] = new Station(StationNames[i], null, null, null); // Fills in the name of the station
            string[] StationInfo = File.ReadAllLines(_Stations[i].stblPath); // Gets all information from the .stbl station file
        }
        Stations = _Stations;

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

        for (int i = 0; i < Stations.Length; i++)
        {
            try
            {
                string[] StationInfo = File.ReadAllLines(Stations[i].stblPath); // Gets all information from the .stbl station file
                string[] AdjacentStationNames = StationInfo[0].Split(';');
                Stations[i].AdjacentStations = Station.NamesToStations(AdjacentStationNames);

                // Console.WriteLine("Imported " + Stations[i].Name + " Station Data"); // In case of debug, un-comment this line of code.
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Skipped Station " + Stations[i].Name);
            }
            //catch ()
            //{
            //
            //}
            
        }
        
        Console.WriteLine("Importing Route Data...");
        Routes = Route.Import();

        Console.WriteLine("----------------------------------------------------------------");
        Console.WriteLine("SCR Realism Project v1.9.7 Build 2");
        Console.WriteLine("Developed by Eve");
        Console.WriteLine("Enter \"help\" or \"commands\" to get a list of commands.");
        Console.WriteLine("----------------------------------------------------------------");
        Tab();
        Console.WriteLine("Waiting for input...");

        Tab();
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
                case "commnds":

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
                            if (!Contains((EnteredCommand[2]), StationNames)) // Find a way to get station names that are 2 or more words like "City Hospital" into one argument
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            if (EnteredCommand[3] == "all")
                            {
                                Station.NameToStation(EnteredCommand[2]).PrintDepartures();
                                break;
                            }
                            if ((EnteredCommand[3] != "all") && (EnteredCommand.Length < 5))
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            /*
                            else // Find a way to automatically see if something is in time format or in frames
                            {

                            }
                            */

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
}
