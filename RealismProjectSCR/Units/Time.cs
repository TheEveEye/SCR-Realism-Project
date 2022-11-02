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
        public static readonly int FormatFrames = 0;
        public static readonly int FormatTime = 1;
        public static readonly int FormatSeconds = 2;
        public static readonly int FormatInvalid = 3;
        
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
            if ((double)ts.TotalDays >= 1) throw new FormatException("TimeSpan crossed limit of 24 hours: " + Math.Round((double)Frames / 4 / 60, 2) + "h");
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
            if ((units.Length <= 2) !& (units.Length >= 3))
            {
                throw new ArgumentException("Not enough Arguments given");
            }
            int hours = Convert.ToInt32(units[0]) * 60 * 60;
            int minutes = Convert.ToInt32(units[1]) * 60;
            if (units.Length == 3)
            {
                int seconds = Convert.ToInt32(units[2]);
                return seconds + minutes + hours;
            }
            else
            {
                return minutes + hours;
            }
        }
        
        public static int GetTimeFormat(string time)
        {
            int type = 3;
            
            string[] StringTimeCheck = time.Split(':');
            if (StringTimeCheck.Length != 3)
            {
                try 
                { 
                    int Time = Convert.ToInt32(time); 
                    if ((Time >= 5760) && (Time < 86400))
                    {
                        type = 2;
                    }
                    if ((Time >= 0) && (Time < 5760))
                    {
                        type = 1;
                    }
                    else
                    {
                        type = 3;
                    }
                }
                catch (System.FormatException) {type = 3;}
            }
            else
            {
                try
                {
                    Convert.ToInt32(StringTimeCheck[0]);
                    Convert.ToInt32(StringTimeCheck[1]);
                    Convert.ToInt32(StringTimeCheck[2]);
                    type = 1;
                }
                catch (System.FormatException) {type = 3;}
            }
            return type;
        }

        public static long UnixNow()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
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

        public int ToFrames()
        {
            return End - Start;
        }

        public static bool Conflicts(TimeFrame timeFrame1, TimeFrame timeFrame2)
        {
            bool output = false;

            if ((timeFrame1.Start >= timeFrame2.Start) || (timeFrame1.Start <= timeFrame2.End))
            {
                output = true;
            }
            if ((timeFrame1.End >= timeFrame2.Start) || (timeFrame1.End <= timeFrame2.End))
            {
                output = true;
            }
            return output;
        }
    }
}
