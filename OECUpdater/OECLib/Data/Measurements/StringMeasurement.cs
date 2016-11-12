using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data.Measurements
{
	public class StringMeasurement : Measurement
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

		public override MeasurementUnit getValue(){
			return new MeasurementUnit (value);
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) 
				return false;

			StringMeasurement measure = (StringMeasurement)obj;
			return measure.value == value;
		}

        public void setValue(String name)
        {
            this.value = name;
        }

    }
}
