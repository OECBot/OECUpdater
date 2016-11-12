using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data.Measurements
{
    internal class NumberMeasurement : Measurement
    {
        double measurement;

        public NumberMeasurement(string name, double measurement) : base(name)
        {
            this.measurement = measurement;
            GetMeasurementType = MeasurementType.NumberMeasurement;
        }

        public override XmlElement WriteXmlTag(XmlElement element)
        {
            element.InnerText = measurement.ToString();
            return element;
        }

		public override MeasurementUnit getValue(){
			return new MeasurementUnit (measurement);
		}

        public void setValue(double value)
        {
            this.measurement = value;
        }


    }
}
