﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.Units;

namespace RealismProjectSCR.SCRObjects.TimeTables
{
    public class Departure
    {
        public int Frame { get; set; }
        public Station Station { get; set; }
        public Platform[] PossiblePlatforms { get; set; }
        public Station Terminus { get; set; }
        public Route Route { get; set; }

        public Departure(int Frame, Station Station, Platform[] PossiblePlatforms, Station Terminus, Route Route)
        {
            this.Frame = Frame;
            this.Station = Station;
            this.PossiblePlatforms = PossiblePlatforms;
            this.Terminus = Terminus;
            this.Route = Route;
        }

        public string ToDebug()
        {
            return Time.ScheduleFramesToDateTime(Frame) + " / " + Frame + " - " + Terminus.Name + " - Platforms: " + PossiblePlatforms.ToString();
        }

        public string ToPassenger()
        {
            return Time.ScheduleFramesToDateTime(Frame) + " - " + Terminus.Name + " - Platforms: " + PossiblePlatforms.ToString();
        }
    }
}
