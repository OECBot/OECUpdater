using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data.Measurements
{
    public enum MeasurementType {  StringMeasurement, NumberMeasurement, NumberErrMeasurement }

    public abstract class Measurement
    {
        public Measurement(string name)
        {
            MeasurementName = name;
        }

        public string MeasurementName
        {
            get;
        }

        public MeasurementType GetMeasurementType
        {
            get;
            protected set;
        }

		public abstract MeasurementUnit getValue ();
        public abstract XmlElement WriteXmlTag(XmlElement element);

    }
}
