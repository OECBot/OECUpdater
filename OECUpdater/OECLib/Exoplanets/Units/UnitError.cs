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
        public String value { get; set;}
        public String type { get; set; }
        public String name { get; set; }
        public String errorPlus {get; set;}
        public String errorMinus { get; set; }

        public UnitError(String name, String value, String errorplus="", String errorminus="", String type="")
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
            if (errorMinus != "")
            {
                w.WriteAttributeString("errorminus", errorMinus);
            }
            if (errorPlus != "")
            {
                w.WriteAttributeString("errorplus", errorPlus);
            }
            if (type != "")
            {
                w.WriteAttributeString("type", type);
            }
            w.WriteString(value);
            w.WriteEndElement();
        }
    }
}
