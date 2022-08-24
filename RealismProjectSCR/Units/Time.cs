using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;

namespace RealismProjectSCR.Units
{
    public class Time
    {

        public static int ElapsedSeconds()
        {
            int seconds = DateTime.UtcNow.Second;
            int minutes = DateTime.UtcNow.Minute * 60;
            int hours = DateTime.UtcNow.Hour * 60 * 60;

            return seconds + minutes + hours;
        }

        public static int ElapsedFrames()
        {
            return ElapsedSeconds() / 15;
        }

        public static DateTime ScheduleFramesToDateTime(int Frames)
        {
            TimeSpan ts = TimeSpan.FromSeconds(Frames * 15);
            return Convert.ToDateTime(ts.ToString());
        }
        public static DateTime SecondsToDateTime(int Frames)
        {
            TimeSpan ts = TimeSpan.FromSeconds(Frames);
            return Convert.ToDateTime(ts.ToString());
        }
        public static int TimeToSeconds(string Time)
        {
            string[] units = Time.Split(':');

            int hours = Convert.ToInt32(units[0]) * 60 * 60;
            int minutes = Convert.ToInt32(units[1]) * 60;
            int seconds = Convert.ToInt32(units[2]);

            return seconds + minutes + hours; 
        }
    }

    public struct TimeFrame
    {
        public int Start { get; set; } // Meassured in ScheduleFrames
        public int End { get; set; }// Meassured in ScheduleFrames
        public string StartTime { get; set; } // Meassured in Time (e. g. 13:45:15)
        public string EndTime { get; set; } // Meassured in Time (e. g. 13:45:15)

        public TimeFrame(int Start, int End)
        {
            this.Start = Start;
            this.End = End;
            this.StartTime = Time.ScheduleFramesToDateTime(Start).ToLongTimeString();
            this.EndTime = Time.ScheduleFramesToDateTime(End).ToLongTimeString();

        }
        public TimeFrame(string StartTime, string EndTime)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.Start = Time.TimeToSeconds(StartTime);
            this.End = Time.TimeToSeconds(EndTime);
        }
    }
}
