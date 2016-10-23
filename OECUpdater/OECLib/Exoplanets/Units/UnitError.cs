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
        public String value {get; set;}
        public string name { get; set; }
        public String errorPlus {get; set;}
        public String errorMinus { get; set; }

        public UnitError(string name, String value, String errorplus, String errorminus)
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

        public void Write(XmlWriter w)
        {
            w.WriteStartElement(name);
            w.WriteAttributeString("errorminus", errorMinus);
            w.WriteAttributeString("errorplus", errorPlus);
            w.WriteString(value);
            w.WriteEndElement();
        }
    }
}
