using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data
{
    public enum MeasurementType {  StringMeasurement, NumberMeasurement, NumberErrMeasurement }

    public class Measurement
    {
		
		public Measurement(string name, string value, Dictionary<string, string> attributes=null)
        {
            MeasurementName = name;
			MeasurementValue = value;
			MeasurementAttributes = attributes;
        }

        public string MeasurementName {
			get;
			private set;
        }
			
		public string MeasurementValue {
			get;
			private set;
		}

		public Dictionary<string, string> MeasurementAttributes {
			get;
			private set;
		}

		public XmlElement WriteXmlTag(XmlElement element) 
		{
			element.InnerText = MeasurementValue.ToString();
			foreach (string key in MeasurementAttributes.Keys) {
				element.SetAttribute (key, MeasurementAttributes [key]);
			}
			return element;
		}

    }
}
