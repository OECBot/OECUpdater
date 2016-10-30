using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data.Measurements
{
    internal class StringMeasurement : Measurement
    {
        string value;

        public StringMeasurement(string name, string value) : base(name)
        {
            this.value = value;
            GetMeasurementType = MeasurementType.StringMeasurement;
        }

        public override XmlElement WriteXmlTag(XmlElement element)
        {
            element.InnerText = value.ToString();
            return element;
        }


    }
}
