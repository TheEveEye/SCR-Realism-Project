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
        static readonly long ApplicationId = 1035200601254023208;
        static Discord.Discord discord = new Discord.Discord(ApplicationId, (UInt64)Discord.CreateFlags.Default);
        static Discord.ActivityManager discordActivityManager = discord.GetActivityManager();

        Discord.Activity startupActivity = new Discord.Activity()
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
    }
    class State
    {
        public string Message { get; set; }
        public StateType Type { get; set; }
        public Leg? AddedLeg { get; set; }

        public static State Idle = new State()
        {
            Message = "Idle",
            Type = StateType.Idle,

        };
        public static State LegAdd = new State()
        {
            AddedLeg = Leg.LastCreatedLeg(),
            Message = String.Format("Added new leg on {0}", LegAdd.AddedLeg.Route.ToNumberNameOutput()),
            Type = StateType.LegAdd,
        };

        public enum StateType
        {
            Idle = 0,
            LegAdd = 1,
            LegView = 2,
            LegEdit = 3,
            LegOther = 4,
            Station = 5,
            DriverAdd = 6,
            DriverView = 7,
            DriverEdit = 8,
            RouteEdit = 9,
            RouteView = 10,
            Platform = 11,
            Other = 12,
            Custom = 13,
        }
    }
}
