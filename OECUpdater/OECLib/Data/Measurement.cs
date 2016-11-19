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
			element.InnerText = MeasurementValue.ToString ();
			if (MeasurementAttributes != null) {
				foreach (string key in MeasurementAttributes.Keys) {
					element.SetAttribute (key, MeasurementAttributes [key]);
				}
			}
			return element;
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			Measurement objMeasure = (Measurement)obj;
			if (objMeasure.MeasurementName == MeasurementName)
				return true;
			return false;
		}

		public bool AttributesAreEqual(Measurement measurement) {
			Dictionary<string,string> otherAttr = measurement.MeasurementAttributes;
            if (otherAttr != null)
            {
                foreach (string key in MeasurementAttributes.Keys)
                {
                    if (otherAttr.ContainsKey(key))
                    {
                        if (otherAttr[key] != MeasurementAttributes[key])
                            return false;
                    }
                }
            }
			
			return true;
		}
    }
}
