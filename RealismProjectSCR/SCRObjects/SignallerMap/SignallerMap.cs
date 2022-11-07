using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RealismProjectSCR;
using RealismProjectSCR.SCRObjects;
using RealismProjectSCR.Units;
using RealismProjectSCR.SCRObjects.TimeTables;
using RealismProjectSCR.SCRObjects.SignallerMap;

namespace RealismProjectSCR.SCRObjects.SignallerMap
{
    public class SignallerMap
    {
        public SignalNode[] SignalNodes { get; set; }
        public TrackNode[] TrackNodes { get; set; }
        public Segment[] TrackSegments { get; set; }
        
        public SignallerMap()
        {
            return Import()
        }
        static SignallerMap Import()
        {
            
        }
    }
}
