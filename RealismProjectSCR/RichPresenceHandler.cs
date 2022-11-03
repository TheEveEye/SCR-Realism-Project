using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.Units;

namespace RealismProjectSCR
{
    public class RichPresenceHandler
    {
        static bool isDiscordClientRunning;
        static bool isDiscordClientInstalled;
        static readonly long ApplicationId = 1035200601254023208;
        static Discord.Discord discord = new Discord.Discord(ApplicationId, (UInt64)Discord.CreateFlags.Default);
        static Discord.ActivityManager discordActivityManager = discord.GetActivityManager();

        public static Discord.Activity currentActivity;

        public static Discord.Activity templateActivity = new Discord.Activity()
        {
            ApplicationId = ApplicationId, // Application ID
            Name = "Realism Project Network Planner", // Name of the Application
            Timestamps = new Discord.ActivityTimestamps()
            {
                Start = Program.ProgramStartUnix
            },
            Assets = new Discord.ActivityAssets()
            {
                LargeImage = "networkplannericon"
            },
            Instance = false
        };
        public static Discord.Activity startupActivity = new Discord.Activity()
        {
            ApplicationId = ApplicationId, // Application ID
            Name = "Realism Project Network Planner", // Name of the Application
            State = "Idle", // Last command executed (e. g. Created new leg (R054))
            Details = "Opening Project", // Name of active Shift (this.ActiveShift.Name)
            Timestamps = new Discord.ActivityTimestamps()
            {
                Start = Time.UnixNow()
            },
            Assets = new Discord.ActivityAssets()
            {
                LargeImage = "networkplannericon"
            },
            Instance = false,
        };

        public static Discord.Result UpdateActivity(Discord.Activity discordActivity)
        {
            if (!isDiscordClientInstalled) return Discord.Result.NotInstalled;
            if (!isDiscordClientRunning) return Discord.Result.NotRunning;
            if (currentActivity.Equals(discordActivity)) return Discord.Result.Ok;
            
            currentActivity = discordActivity;

            Discord.Result hasUpdated = Discord.Result.LobbyFull;
            discordActivityManager.UpdateActivity(discordActivity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    hasUpdated = result;
                }
            });
            while ((hasUpdated != Discord.Result.Ok) && (hasUpdated == Discord.Result.LobbyFull))
            {
                discord.RunCallbacks();
            }
            if (hasUpdated == Discord.Result.NotInstalled)
            {
                isDiscordClientInstalled = false;
                isDiscordClientRunning = false;
            }
            if (hasUpdated == Discord.Result.NotRunning)
            {
                isDiscordClientInstalled = true;
                isDiscordClientRunning = false;
            }
            return hasUpdated;
        }
        public static Discord.Result UpdateActivity(string state)
        {
            if (!isDiscordClientInstalled) return Discord.Result.NotInstalled;
            if (!isDiscordClientRunning) return Discord.Result.NotRunning;

            Discord.Activity activity = templateActivity;
            activity.State = state;
            activity.Details = Program.ActiveShift.Name;

            return UpdateActivity(activity);
        }

        public static void Setup()
        {
            System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
            isDiscordClientInstalled = true;
            isDiscordClientRunning = true;

            var result = UpdateActivity(startupActivity);
            currentActivity = startupActivity;
            if (result == Discord.Result.Ok)
            {
                Console.WriteLine("Connected!");
            }
            else
            {
                Console.WriteLine("Failed to connect");
            }
        }
    }
}
