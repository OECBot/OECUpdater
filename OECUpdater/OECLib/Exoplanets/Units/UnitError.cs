using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECLib.Exoplanets.Units
{
    public class UnitError
    {
        public double value {get; set;}
        public double error {get; set;}

        public UnitError(double value, double error)
        {
            this.value = value;
            this.error = error;
        }
    }
}
