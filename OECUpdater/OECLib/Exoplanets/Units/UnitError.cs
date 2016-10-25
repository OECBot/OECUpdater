using OECLib.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Exoplanets.Units
{
    public class UnitError : XMLWritable
    {
        public String name { get; set; }
        public object value { get; set;}
        //public String type { get; set; }
        public Double errorPlus {get; set;}
        public Double errorMinus { get; set; }

        public UnitError(String name, object value, Double errorplus=0.0, Double errorminus=0.0)
        {
            this.name = name;
            this.value = value;
            //this.type = type;
            this.errorPlus = errorplus;
            this.errorMinus = errorminus;
        }

        public override string ToString()
        {
            return string.Format("<{0} errorminus=\"{1}\" errorplus=\"{2}\">{3}</{0}>", name, errorMinus, errorPlus, value);
        }

        public void Write(XmlWriter w)
        {
            if (value is IList<object>)
            {
                foreach (object val in (IList)value)
                {
                    WriteElement(w, val);
                }
            }
            else
            {
                WriteElement(w, value);
            }
        }

        private void WriteElement(XmlWriter w, object val)
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
            //if (type != "")
            //{
            //    w.WriteAttributeString("type", type);
            //}
            w.WriteString(val.ToString());
            w.WriteEndElement();
        }
    }
}
