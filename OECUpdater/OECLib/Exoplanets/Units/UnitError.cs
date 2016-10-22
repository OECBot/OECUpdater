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
        public string name { get; set; }
        public double errorPlus {get; set;}
        public double errorMinus { get; set; }

        public UnitError(string name, double value, double errorplus, double errorminus)
        {
            this.name = name;
            this.value = value;
            this.errorPlus = errorplus;
            this.errorMinus = errorminus;
        }

        public override string ToString()
        {
            return string.Format("<{0} errorminus=\"{1}\" errorplus=\"{2}\">{3}</{0}>", name, errorMinus, errorPlus, value);
        }
    }
}
