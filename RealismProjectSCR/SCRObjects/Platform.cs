using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class Platform
    {
        public int PlatformNumber { get; }
        public bool IsAllocated { get; set; }
        public Route[] StoppingRoutes { get; set; }
        
        public Platform(int PlatformNumber, bool IsAllocated, Route[] StoppingRoutes)
        {
            this.PlatformNumber = PlatformNumber;
            this.IsAllocated = IsAllocated;
            this.StoppingRoutes = StoppingRoutes;
        }
        public static string PlatformsToString(Platform[] Platforms)
        {
            string output = Convert.ToString(Platforms[0].PlatformNumber);
            foreach (var Number in Platforms)
            {
                output += (", " + Number.PlatformNumber);
            }
            return output;
        }
    }
}
