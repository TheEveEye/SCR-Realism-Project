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
        public MergeNode[] MergeNodes { get; set; }
        public SplitNode[] SplitNodes { get; set; }
        public CrossNode[] CrossNodes { get; set; }
        public JunctionNode[] JunctionNodes { get; set; }
        public TerminusNode[] TerminusNodes { get; set; }
        public Segment[] TrackSegments { get; set; }
        
        public SignallerMap()
        {
           Import();
        }
        static SignallerMap Import()
        {
            return null;
        }
    }
}
