using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR;
using RealismProjectSCR.NetworkPlanner;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.Units;

namespace RealismProjectSCR.SCRObjects
{
    public class Headcode
    {
        public string Code { get; set; }
        public int Priority { get; set; }
        public char Terminus { get; set; }
        public int Number { get; set; }

        public Headcode(int Priority, char Terminus, int Number)
        {
            this.Code = Convert.ToString(Priority) + Terminus + Convert.ToString(Number);

            this.Priority = Priority;
            this.Terminus = Terminus;
            this.Number = Number;
        }

    }
}
