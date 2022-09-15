﻿using System;
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
        public List<Leg> Legs { get; set; }
        public string Path { get; set; }
        
        public Shift(string Name, TimeFrame TimeFrame, List<Leg> Legs)
        {
            this.Name = Name;
            this.TimeFrame = TimeFrame;
            this.Legs = Legs;
            this.Path = GetPath(this.Name); 
        }
        public static string GetPath(string Name)
        {
            return Program.ProjectDirectoryPath + @"Shifts\"+ Name + @"\";
        }
        public void AddLeg(Leg leg)
        {
            Legs.Add(Leg);
        }
        public static Shift Import(string Name)
        {
            Shift output = new(null, new TimeFrame(null, null), null);



            return output;
        }
        public static void Create(Shift Shift)
        {
            
        }
    }
}
