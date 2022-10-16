using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealismProjectSCR.SCRObjects;

namespace RealismProjectSCR.SCRObjects
{
    public class Platform
    {
        public string PlatformName { get; set; }
        public bool IsAllocated { get; set; }
        public List<Leg> Legs { get; set; }
        public Route[] StoppingRoutes { get; set; }
        public Operator[] Operators { get; set; }
        
        
        public Platform(string PlatformName, bool IsAllocated, List<Leg> Legs, Route[] StoppingRoutes, Operator[] Operators)
        {
            this.PlatformName = PlatformName;
            this.IsAllocated = IsAllocated;
            this.Legs = Legs;
            this.StoppingRoutes = StoppingRoutes;
            this.Operators = Operators;
        }
        public static string PlatformsToString(Platform[] Platforms)
        {
            string output = Convert.ToString(Platforms[0].PlatformName);
            foreach (var Number in Platforms)
            {
                output += (", " + Number.PlatformName);
            }
            return output;
        }
    }
}
