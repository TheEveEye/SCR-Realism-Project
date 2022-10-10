using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealismProjectSCR.SCRObjects
{
    public class Operator
    {
        public static Operator Connect = new Operator("Connect");
        public static Operator Express = new Operator("Express");
        public static Operator Waterline = new Operator("Waterline");
        public static Operator Airlink = new Operator("Airlink");

        public string Name { get; set; }
        
        public Operator(string Name)
        {
            this.Name = Name;
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
