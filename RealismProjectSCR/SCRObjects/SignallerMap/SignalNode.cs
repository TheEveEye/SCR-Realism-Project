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
    public class SignalNode
    {
        public Type SignalType { get; set; }
        public Aspect SignalAspect { get; set; }
        public int ID { get; set; }
        public Segment[] AdjacentSegments { get; set; }
        
        enum Type
        {
            4Aspect = 0,
            3Aspect = 1,
            2Aspect = 2,
            Shunt = 3
        }
        enum Aspect
        {
            Proceed = 0,
            PreliminaryCaution = 1,
            Caution = 2,
            Danger = 3
        }
        
        public Signal(Type SignalType, Aspect SignalAspect, int ID, Segment[] AdjacentSegments)
        {
            this.SignalType = SignalType;
            this.SignalAspect = SignalAspect;
            this.ID = ID;
            this.AdjacentSegments = AdjacentSegments;
        }
    }
}
