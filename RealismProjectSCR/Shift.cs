using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.NetworkPlanner;
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
        public List<Leg> Legs { get; set; }
        public string Path { get; set; }

        
        public Shift(string Name, TimeFrame TimeFrame, string Description, List<Leg> Legs)
        {
            this.Name = Name;
            this.TimeFrame = TimeFrame;
            this.Description = Description;
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
        public void Push()
        {
            string[] vs = new string[Legs.Count];
            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] = Leg.ToExport(Legs[i]);
            }
            File.WriteAllLines(GetPath(this.Name) + @"Legs.shift", vs);

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
        public static Shift Import(string Path)
        {
            Shift output = new(NameFromPath(Path), new TimeFrame(0, 0), new List<Leg>());

            string[] shiftInfo = File.ReadAllLines(Path + @"\Info.shift");
            string[] timeFrameRaw = shiftInfo[0].Split(';');
            output.TimeFrame = new TimeFrame(Convert.ToInt32(timeFrameRaw[0]), Convert.ToInt32(timeFrameRaw[1]));
            string[] legsRaw = File.ReadAllLines(Path + @"\Legs.shift");
            Leg[] legs = new Leg[legsRaw.Length];
            for (int i = 0; i < legs.Length; i++)
            {
                legs[i] = Leg.Import(legsRaw[i]);
            }
            output.Legs = legs.ToList<Leg>();
            return output;
        }
        public static Shift Create(string startingTime, string endingTime, string shiftName, string description)
        {
            return Create(Time.TimeToScheduleFrame(startingTime), Time.TimeToScheduleFrame(endingTime), shiftName, description)
        }
        
        public static Shift Create(int startingFrame, int endingFrame, string shiftName, string description)
        {
            return new Shift(shiftName, new TimeFrame(startingFrame, endingFrame), description, new List<Leg>)
        }
        public string[] LegsToDebug()
        {
            List<string> output = new List<string>();
            for (int i = 0; i < Legs.Count; i++)
            {
                output.Add(Legs[i].ToCompact(i + 1));
            }
            return output.ToArray();
        }
    }
}
