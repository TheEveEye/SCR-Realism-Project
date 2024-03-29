﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.Units;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR;

namespace RealismProjectSCR.SCRObjects.TimeTables
{
    public class Departure
    {
        public int Frame { get; set; }
        public Station Station { get; set; }
        public Platform[] PossiblePlatforms { get; set; }
        public Station Terminus { get; set; }
        public Route Route { get; set; }
        public Headcode Headcode { get; set; }
        public TimeFrame Occupation { get; set; }

        public Departure(int Frame, Station Station, Platform[] PossiblePlatforms, Station Terminus, Route Route, Headcode Headcode)
        {
            this.Frame = Frame;
            this.Station = Station;
            this.PossiblePlatforms = PossiblePlatforms;
            this.Terminus = Terminus;
            this.Route = Route;
            this.Headcode = Headcode;
            this.Occupation = new TimeFrame(this.Frame - this.Route.Operator.MinimumThreshold - 2, this.Frame + 2);
        }

        public string ToDebug()
        {
            if (PossiblePlatforms == null)
            {
                if (this.Terminus == this.Station)
                {
                    return "(Arrival) " + Route.RouteNumberString(this.Route.RouteNumber) + "    " + Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " / " + Frame + " - " + Terminus.Name; 
                }
                return Route.RouteNumberString(this.Route.RouteNumber) + "    " + Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " / " + Frame + " - " + Terminus.Name;
            }
            else
            {
                return Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " / " + Frame + " - " + Terminus.Name + " - Platforms: " + PossiblePlatforms.ToString();
            }

        }
        public string ToDispatcher()
        {
            if (PossiblePlatforms == null)
            {
                if (this.Terminus == this.Station)
                {
                    return "(Arrival) " + Route.RouteNumberString(this.Route.RouteNumber) + "    " + Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " / " + Frame + " - " + Terminus.Name;
                }
                return Route.RouteNumberString(this.Route.RouteNumber) + "    " + Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " / " + Frame + " - " + Terminus.Name;
            }
            else
            {
                return Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " / " + Frame + " - " + Terminus.Name + " - Platforms: " + PossiblePlatforms.ToString();
            }
        }

        public string ToDriver()
        {
            return Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " - " + Station.Name;
        }
        public string ToPassenger()
        {
            if (this.Terminus == this.Station)
            {
                return Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " - " + "(Arrival) " + Terminus.Name + "    (" + Route.RouteNumberString(this.Route.RouteNumber) + ")";
            }
            return Time.ScheduleFramesToDateTime(Frame).ToLongTimeString() + " - " + Terminus.Name + "    (" + Route.RouteNumberString(this.Route.RouteNumber) + ")";
        }
    }
}
