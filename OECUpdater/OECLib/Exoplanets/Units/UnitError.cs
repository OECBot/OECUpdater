using OECLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Exoplanets.Units
{
    public class UnitError : XMLWritable
    {
        public Double value { get; set;}
        public String type { get; set; }
        public String name { get; set; }
        public Double errorPlus {get; set;}
        public Double errorMinus { get; set; }

        public UnitError(String name, Double value, Double errorplus=0.0, Double errorminus=0.0, String type="")
        {
            this.name = name;
            this.value = value;
            this.type = type;
            this.errorPlus = errorplus;
            this.errorMinus = errorminus;
        }

        public override string ToString()
        {
            return string.Format("<{0} errorminus=\"{1}\" errorplus=\"{2}\">{3}</{0}>", name, errorMinus, errorPlus, value);
        }

        public void Write(XmlWriter w)
        {
            w.WriteStartElement(name);
            if (errorMinus != 0.0)
            {
                w.WriteAttributeString("errorminus", errorMinus.ToString());
            }
            if (errorPlus != 0.0)
            {
                w.WriteAttributeString("errorplus", errorPlus.ToString());
            }
            if (type != "")
            {
                w.WriteAttributeString("type", type);
            }
            w.WriteString(value.ToString());
            w.WriteEndElement();
        }
    }
}
