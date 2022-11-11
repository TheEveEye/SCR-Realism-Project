using RealismProjectSCR;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

class Program
{
    public static Station[] Stations;
    public static Route[] Routes;
    public static string[] StationNames;
    public static Timing[] DepotTimings;
    public static List<string> ShiftPaths;
    public static List<string> ShiftNames;
    public static Shift ActiveShift;

    public static List<string> CommandHistory = new List<string>();
    public static long ProgramStartUnix;

    public static string ProjectDirectoryPath;
    public static string ExePath;

    public static int BuildNumber = 86;

    static void Main(string[] args)
    {
        Console.Title = "Realism Project Network Planner Build " + BuildNumber;

        ProgramStartUnix = Time.UnixNow();


        Console.WriteLine("Importing Program Data...");
        ProjectDirectoryPath = Path.GetFullPath(@"resources\");
        List<string> exePathRaw = ProjectDirectoryPath
            .Split('\\')
            .ToList()
            .GetRange(0, ProjectDirectoryPath
            .Split('\\').Length - 1);
        exePathRaw.Add("RealismProjectSCR.exe");
        ExePath = BuildString(exePathRaw.ToArray(), "\\");

        Console.WriteLine("Importing Station Data...");
        List<Station> _Stations = new List<Station>(); // Creating List, then converted to array later
        StationNames = File.ReadAllLines(ProjectDirectoryPath + @"SCRObjects\StationList.txt");

        string[] AdjacentStations = File.ReadAllLines(ProjectDirectoryPath + @"SCRObjects\AdjacentStations.txt");
        for (int i = 0; i < StationNames.Length; i++) // This for-loop 
        {
            _Stations.Add(new Station(StationNames[i], null, null, new List<Departure>())); // Fills in the name of the station
            _Stations[i].GetSetShortcuts();
        }
        Stations = _Stations.ToArray();
        for (int i = 0; i < Stations.Length; i++)
        {
            Stations[i].AdjacentStations = Station.NamesToStations(AdjacentStations[i].Split(':')[1].Split(';'));
        }

        Console.WriteLine("Importing Route Data...");
        Routes = Route.Import();

        Console.WriteLine("Importing Shift Data...");
        ShiftPaths = GetShiftPaths();
        ShiftNames = Shift.NamesFromPaths(ShiftPaths.ToArray()).ToList<string>();

        // Starting up Discord Rich Presence Client
        Console.WriteLine("Connecting with Discord RPC...");
        RichPresenceHandler.Setup(); // Sets up RichPresence, and sets the current status to Opening Project, Idle

        Console.WriteLine("----------------------------------------------------------------");
        Console.WriteLine("SCR Realism Project Network Planner v1.10.2 Build " + BuildNumber);
        Console.WriteLine("Developed by Eve                                                ");
        Console.WriteLine("Enter \"help\" or \"commands\" to get a list of commands.       ");
        Console.WriteLine("----------------------------------------------------------------");

        bool selectedShift = false;
        while (!selectedShift)
        {
            try
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
                    selectedShiftIndex = Convert.ToInt32(selectedShiftInput);
                    if (selectedShiftIndex == ShiftNames.Count + 1)
                    {
                        ActiveShift = Shift.Create(Shift.Collect());
                        RichPresenceHandler.UpdateActivity(String.Format("Created new shift named {0}", ActiveShift.Name));
                        ShiftNames.Add(ActiveShift.Name);
                        ShiftPaths.Add(ActiveShift.Path);
                    }
                    else
                    {
                        try
                        {
                            ActiveShift = Shift.Import(ShiftPaths[selectedShiftIndex - 1], ShiftNames[selectedShiftIndex - 1]);
                            RichPresenceHandler.UpdateActivity("Idle");
                            ActiveShift.PredictHeadcodes();
                            selectedShift = true;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("This shift does not exist. Please try again...");
                        }    
                    
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Command. Please try again...");
            }
        }

        Console.WriteLine("Waiting for input...");

        bool proceed = false; // Use for all menu checks
        bool close = false;
        string EnteredCommandRaw = " ";

        while (!close)
        {
            Tab();
            while (!proceed)
            {
                EnteredCommandRaw = Console.ReadLine();

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

            string[] EnteredCommand = EnteredCommandRaw.ToLower().Split(' '); // Splitting up entered command into each arguement
            CommandHistory.Add(EnteredCommandRaw);

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
                    Console.WriteLine("The \"help\" command is currently not implemented. We are looking forward to adding this command in the near future.");
                    /* Remove these comments when the "help" command gets implemented.
                    if (EnteredCommand.Length == 1)
                    {

                    }
                    else if (EnteredCommand.Length > 1)
                    {
                        switch (EnteredCommand[1])
                        {
                            default:
                                break;
                        }
                    }
                    */
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
                        case "driver":
                            if (EnteredCommand.Length < 5)
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
                                Convert.ToInt32(EnteredCommand[2]); // Route
                                Convert.ToInt32(EnteredCommand[3]); // Spawning Frame
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            RichPresenceHandler.UpdateActivity(String.Format("Created new driver on {0}", Route.RouteNumberString(Convert.ToInt32(EnteredCommand[2]))));
                            Route driverRoute = Routes[Convert.ToInt32(EnteredCommand[2]) - 1];
                            int spawningFrame = Convert.ToInt32(EnteredCommand[3]);
                            string driverName = EnteredCommand[4];
                            Driver output = new Driver(driverRoute, spawningFrame, new List<Leg>(), driverName);
                            ActiveShift.Drivers.Add(output);
                            Driver.Export(ActiveShift.Drivers);
                            break;

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
                                var tempDriver = ActiveShift.Drivers[Convert.ToInt32(EnteredCommand[2])]; // Driver Number
                                var tempRoute = Routes[Convert.ToInt32(EnteredCommand[3]) - 1]; // Route
                                Time.ScheduleFramesToDateTime(Convert.ToInt32(EnteredCommand[4]) + ActiveShift.TimeFrame.Start); // Starting Frame
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            Route route = Routes[Convert.ToInt32(EnteredCommand[3]) - 1];
                            RichPresenceHandler.UpdateActivity(String.Format("Created new leg on {0}", Route.RouteNumberString(route.RouteNumber)));
                            Driver driver = ActiveShift.Drivers[Convert.ToInt32(EnteredCommand[2])];
                            int StartingFrame = Convert.ToInt32(EnteredCommand[4]);
                            Station startingStation = Station.FromArgument(EnteredCommand[5]);
                            Station endingStation = Station.FromArgument(EnteredCommand[6]);
                            if ((startingStation == null) || (endingStation == null))
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            Leg createdLeg = Leg.Create(route, StartingFrame + ActiveShift.TimeFrame.Start, startingStation, endingStation, driver);
                            driver.Legs.Add(createdLeg);
                            ActiveShift.AddLeg(createdLeg);
                            break;

                        case "shift":

                            Shift tempShift = Shift.Create(Shift.Collect());
                            RichPresenceHandler.UpdateActivity(String.Format("Created new shift named {0}", tempShift.Name));
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
                            RichPresenceHandler.UpdateActivity("Viewing the time");
                            break;

                        case "elapsedframes":
                        case "elapsedscheduleframes":
                            Console.WriteLine(Time.ElapsedFrames()); // Gets the amount of schedule frames that have passed today
                            RichPresenceHandler.UpdateActivity("Viewing the time");
                            break;

                        case "drivers":
                            Console.WriteLine(BuildString(Driver.ToCompacts(ActiveShift.Drivers.ToArray()), "\n"));
                            RichPresenceHandler.UpdateActivity("Looking at the Drivers List");
                            break;

                        case "commandhistory":
                            if (CommandHistory.Count == 0)
                            {
                                Console.WriteLine("You have not executed any commands yet.");
                                break;
                            }
                            Console.WriteLine("You have executed " + CommandHistory.Count + " commands this session:");
                            Console.WriteLine(BuildString(CommandHistory.ToArray(), "\n"));
                            break;

                        case "stationtable":
                        case "stationschedule":
                            Station searchedStation = Station.FromArgument(EnteredCommand[2]);
                            if (searchedStation == null) // Find a way to get station names that are 2 or more words like "City Hospital" into one argument
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            RichPresenceHandler.UpdateActivity(String.Format("Looking at the timetable of {0}", searchedStation.Name));
                            searchedStation.SortDepartures();
                            Console.WriteLine(searchedStation.ToStationTable());
                            /*
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
                            */
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
                            RichPresenceHandler.UpdateActivity(String.Format("Looking at Route data of {0}", Route.RouteNumberString(Convert.ToInt32(EnteredCommand[2]))));
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
                            if (Convert.ToInt32(EnteredCommand[2]) < 1)
                            {
                                Console.WriteLine("Invalid leg given. Please enter a different leg index.");
                                break;
                            }
                            ActiveShift.PredictHeadcodes();
                            RichPresenceHandler.UpdateActivity(String.Format("Looking at leg data of a {0} leg", Route.RouteNumberString(ActiveShift.Legs[Convert.ToInt32(EnteredCommand[2]) - 1].Route.RouteNumber)));
                            Console.WriteLine(ActiveShift.Legs[Convert.ToInt32(EnteredCommand[2]) - 1].ToDriver());

                            break;

                        case "legs":
                        case "leglist":
                            string[] compactLegs = ActiveShift.LegsToDebug();
                            ActiveShift.PredictHeadcodes();
                            RichPresenceHandler.UpdateActivity(String.Format("Looking at a list of {0} legs", ActiveShift.Legs.Count));
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
                    RichPresenceHandler.UpdateActivity("Converting Units");
                    switch (EnteredCommand[1]) // Looks at the second argument given
                    {
                        case "seconds":
                            if ((Time.GetTimeFormat(EnteredCommand[3]) != Time.Format.Seconds) && (Time.GetTimeFormat(EnteredCommand[3]) != Time.Format.Frames))
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
                            if (Time.GetTimeFormat(EnteredCommand[3]) != Time.Format.Frames)
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
                            if (Time.GetTimeFormat(EnteredCommand[3]) != Time.Format.Time)
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

                case "edit":
                case "change":
                case "modify":
                case "set":

                    if (EnteredCommand.Length < 3)
                    {
                        Console.WriteLine("Not enough Arguments given. Please try again...");
                        break;
                    }
                    switch (EnteredCommand[1])
                    {
                        case "route":
                            if (EnteredCommand.Length < 5)
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            try
                            {
                                RichPresenceHandler.UpdateActivity(String.Format("Editing Data for {0}", Route.RouteNumberString(Convert.ToInt32(EnteredCommand[2]))));
                                Routes[Convert.ToInt32(EnteredCommand[2]) - 1].EditRoute(EnteredCommand[3], EnteredCommand[4]);
                                Console.WriteLine(String.Format("Successfully modified \"{0}\" in {1} to \"{2}\".", EnteredCommand[3], Route.RouteNumberString(Convert.ToInt32(EnteredCommand[2])), EnteredCommand[4]));
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                                break;
                            }
                            break;

                        case "status":
                        case "customstatus":
                            if (EnteredCommand.Length < 3)
                            {
                                Console.WriteLine("Not enough Arguments given. Please try again...");
                                break;
                            }
                            if (!RichPresenceHandler.isDiscordClientRunning)
                            {
                                Console.WriteLine("Discord Client is either not running or not installed.");
                                break;
                            }
                            switch (EnteredCommand[2])
                            {
                                case "off":
                                case "disable":
                                    RichPresenceHandler.customStatusSet = false;
                                    Console.WriteLine("Turned off custom status.");
                                    break;

                                case "idle":
                                case "afk":
                                    RichPresenceHandler.UpdateActivity("Idle");
                                    break;

                                case "": // No input
                                    Console.WriteLine("Invalid Argument given. Please try again...");
                                    break;

                                default:
                                    var result = RichPresenceHandler.UpdateActivity(BuildString(EnteredCommandRaw.Split(' ').ToList().GetRange(2, EnteredCommandRaw.Split(' ').Length - 2).ToArray(), " "), true);
                                    if (result != Discord.Result.Ok)
                                    {
                                        Console.WriteLine("There seems to be some issue with setting your Custom Status.");
                                    }
                                    Console.WriteLine(String.Format("Successfully changed your status to {0}", EnteredCommand[2]));
                                    break;
                            }
                            break;

                        default: // Not an existing command
                            Console.WriteLine("Invalid Command. Please try again...");
                            break;
                    }
                    break;

                case "delete":
                case "remove":
                case "void":

                switch (EnteredCommand[1])
                {
                    case "leg":
                        Leg selectedLeg = null;
                        int input = -1;
                        bool hasCancelled = false;

                        if (EnteredCommand.Length < 2)
                        {
                            Console.WriteLine("Not enough Arguments given. Please try again...");
                            break;
                        }
                        if (ActiveShift.Legs.Count == 0)
                        {
                            Console.WriteLine("There are no legs to delete...");
                            break;
                            hasCancelled = true;
                        }
                        RichPresenceHandler.UpdateActivity("Deleting legs");
                        if (EnteredCommand.Length == 2) // if no index is specified, print all legs and then choose an index
                        {
                            ActiveShift.PredictHeadcodes();
                            for (int i = 0; i < ActiveShift.Legs.Count; i++)
                            {
                                Console.WriteLine(String.Format("{0} - {1}", i + 1, ActiveShift.Legs[i].ToCompact(i + 1, " - "))); // Prints legs with index
                            }
                            bool validInput = false;
                        
                            while (!validInput)
                            {
                                Console.WriteLine("Select leg: ");
                                string rawInput = Console.ReadLine();
                                if (rawInput == "cancel")
                                {
                                    Console.WriteLine("Okay, cancelled");
                                    hasCancelled = true;
                                    break;
                                }
                                else
                                {
                                    try
                                    {
                                        input = Convert.ToInt32(rawInput) - 1;
                                        selectedLeg = ActiveShift.Legs[input];
                                        validInput = true;
                                    }
                                    catch (System.FormatException)
                                    {
                                        Console.WriteLine("Invalid leg index entered. Please enter a valid number...");
                                    }
                                }
                                if (ActiveShift.Legs.Count < input)
                                {
                                    Console.WriteLine("Invalid Argument given. Please try again...");
                                }
                                if (input < 1)
                                {
                                   Console.WriteLine("Invalid leg given. Please enter a different leg index.");
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                input = Convert.ToInt32(EnteredCommand[2]) - 1;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid Argument given. Please try again...");
                            }
                            selectedLeg = ActiveShift.Legs[input];
                        }
                        if (hasCancelled)
                        {
                            break;
                        }
                        Console.WriteLine("Are you sure that you want to remove leg " + (input + 1) + "?");
                        Console.WriteLine(selectedLeg.ToDriver());
                        bool? confirm = null;
                        while (confirm == null)
                        {
                            string? confirmation = Console.ReadLine();
                            confirm = BoolFromInput(confirmation);
                            if (confirm == null)
                            {
                                Console.WriteLine("Enter \"yes\" or \"no\"...");
                            }
                        }
                        
                        if (confirm == true)
                        {
                            Console.WriteLine("Okay, removing...");
                            ActiveShift.Legs.Remove(selectedLeg);
                            ActiveShift.Push();
                            Console.WriteLine("Removed leg successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Okay, cancelled");
                            hasCancelled = true;
                            break;
                        }
                        break;

                    /*case "driver":
                        Driver selectedDriver = null;
                        int inputIndex = -1;
                        if (EnteredCommand.Length < 2)
                        {
                            Console.WriteLine("Not enough Arguments given. Please try again...");
                            break;
                        }
                        break;*/

                        default: // Not an existing command
                        Console.WriteLine("Invalid Command. Please try again...");
                        break;
                }
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
            "positive",
            "+",
            "affirmative",
            "confirm",
        };
        string[] falseList = new string[]
        {
            "no",
            "false",
            "n",
            "negative",
            "-",
            "dissident",
            "cancel",
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
