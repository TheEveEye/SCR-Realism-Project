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
    public class MergeNode
    {

    }
    public class SplitNode
    {

    }
    public class CrossNode
    {

    }
    public class JunctionNode
    {

    }
    public class TerminusNode
    {

    }
    public class SignalNode
    {
        public Type SignalType { get; set; }
        public Aspect SignalAspect { get; set; }
        public string ID { get; set; }
        public Segment[] AdjacentSegments { get; set; }

        public enum Type
        {
            FourAspect = 0,
            ThreeAspect = 1,
            TwoAspect = 2,
            Shunt = 3
        }
        public enum Aspect
        {
            Proceed = 0,
            PreliminaryCaution = 1,
            Caution = 2,
            Danger = 3
        }

        public SignalNode(Type SignalType, Aspect SignalAspect, string ID, Segment[] AdjacentSegments)
        {
            this.SignalType = SignalType;
            this.SignalAspect = SignalAspect;
            this.ID = ID;
            this.AdjacentSegments = AdjacentSegments;
        }
    }
    public class ZoneConnectionNode
    {
    }
}
