using System;
using System.Collections.Generic;
using System.Collections;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;
using RealismProjectSCR.NetworkPlanner;

class Program
{
    public static string StartupFilePath = Path.GetFullPath("RealismProjectSCR.startup");
    public static string[] StartupFile = File.ReadAllLines(StartupFilePath);
    public static Station[] Stations = new Station[Convert.ToInt32(StartupFile[0])]; // startupFile[0] = amount of stations
    public static string[] StationNames = StartupFile[1].Split(';'); // startupFile[0] = station names
    public static int StationInfoLength = Convert.ToInt32(StartupFile[2]);

    static void Main()
    {
        

        Console.WriteLine("Importing Station Data...");
        for (int i = 0; i < Stations.Length; i++) // This for-loop 
        {
            Stations[i].Name = StationNames[i]; // Fills in the name of the station
            string[] StationInfo = File.ReadAllLines(Stations[i].stblPath); // Gets all information from the .stbl station file
        }
        for (int i = 0; i < Stations.Length; i++)
        {
            string[] StationInfo = File.ReadAllLines(Stations[i].stblPath); // Gets all information from the .stbl station file
            string[] AdjacentStationNames = StationInfo[0].Split(';');
            Stations[i].AdjacentStations = Station.NamesToStations(AdjacentStationNames);

            Console.WriteLine("Imported " + Stations[i].Name + " Station Data");
        }

        Console.WriteLine("----------------------------------------------------------------");
        Console.WriteLine("SCR Realism Project v0.1 Beta");
        Console.WriteLine("Developed by Eve");
        Console.WriteLine("Enter \"help\" or \"commands\" to get a list of commands.");
        Console.WriteLine("----------------------------------------------------------------");
        Tab();
        Console.WriteLine("Waiting for input...");

        Tab();

        Station TestStation = new("Beechly", null, null, null);
        Console.WriteLine(TestStation.stblPath);


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
                            if (EnteredCommand.Length < 3)
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }

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
                            string[] StringTimeCheck = EnteredCommand[3].Split(':');
                            if (StringTimeCheck.Length != 3)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            try
                            {
                                int hours = Convert.ToInt32(StringTimeCheck[0]);
                                int minutes = Convert.ToInt32(StringTimeCheck[1]);
                                int seconds = Convert.ToInt32(StringTimeCheck[2]);
                            }
                            catch (System.FormatException)
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

                default: // Not an existing command
                    Console.WriteLine("Invalid Command. Please try again...");
                    break;
            }
        }
    }

    public static void Tab()
    {
        Console.WriteLine("");
    }
}