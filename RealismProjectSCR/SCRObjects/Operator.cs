using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class Operator
    {
        public static Operator Connect = new Operator("Connect", 2);
        public static Operator Express = new Operator("Express", 4);
        public static Operator Waterline = new Operator("Waterline", 2);
        public static Operator Airlink = new Operator("Airlink", 3);

        public string Name { get; set; }
        public int MinimumThreshold { get; set; }
        
        public Operator(string Name, int MinimumThreshold)
        {
            this.Name = Name;
            this.MinimumThreshold = MinimumThreshold;
        }

        public static Operator FromString(string input)
        {
            if (input == "Connect")
            {
                return Connect;
            }
            else if (input == "Express")
            {
                return Express;
            }
            else if (input == "Waterline")
            {
                return Waterline;
            }
            else if (input == "Airlink")
            {
                return Airlink;
            }
            else
            {
                throw new ArgumentException(String.Format("Unknown Operator \"{0}\"", input));
            }
        }
    }
}
