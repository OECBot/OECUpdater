using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data
{
	public enum StellarType { System, Binary, Star, Planet }

    public abstract class StellarObject
    {
        public List<StellarObject> children;
		public List<Measurement> names;
       	public List<Measurement> measurements;

        public StellarObject()
        {
            children = new List<StellarObject>();
			names = new List<Measurement> ();
			measurements = new List<Measurement>();
        }

		public abstract StellarType ObjectType {
			get;
		}

        public abstract bool AddChild(StellarObject child);

		public abstract bool RemoveChild (StellarObject child);

        public abstract XmlElement XMLTag(XmlDocument root);

        protected void AddMeasurementTags(XmlElement node, XmlDocument root)
        {
            foreach (Measurement name in names)
            {
                XmlElement element = root.CreateElement(name.MeasurementName);
                element = name.WriteXmlTag(element);
                node.AppendChild(element);
            }

            foreach (Measurement measurement in measurements)
            {
                XmlElement element = root.CreateElement(measurement.MeasurementName);
                element = measurement.WriteXmlTag(element);
                node.AppendChild(element);
            }
        }
        
		public override bool Equals(object other) {
			if (other == null | GetType () != other.GetType ())
				return false;

			StellarObject obj = (StellarObject)other;

			//if(names)

			foreach (Measurement thisMeasure in names) {
				foreach (Measurement otherMeasure in obj.names) {
					if (thisMeasure == otherMeasure)
						return true;
				}
			}
			return false;
		}

		public void AddMeasurement(string name, string value, Dictionary<string, string> attributes=null)
		{
			Measurement measurement = new Measurement (name, value, attributes);
				
			if (name == "name") {
				if (!names.Contains (measurement))
					names.Add(measurement);
			} else {
				int index;
				if ((index = measurements.IndexOf(measurement)) == -1)
					measurements.Add (measurement);
				else
					measurements [index] = measurement;
			}
        }

		public void AddMeasurement(Measurement measurement)
		{
			if (measurement.MeasurementName == "name") {
				if (!names.Contains (measurement))
					names.Add (measurement);
			} else {
				int index;
				if ((index = measurements.IndexOf(measurement)) == -1)
					measurements.Add (measurement);
				else
					measurements [index] = measurement;
			}
		}

		public void Write(XmlWriter w)
		{
			if (ObjectType == StellarType.System) {
				w.WriteStartElement("system");
			}
			else if (ObjectType == StellarType.Star) {
				w.WriteStartElement("star");
			}
			else if (ObjectType == StellarType.Binary) {
				w.WriteStartElement("binary");
			}
			else if (ObjectType == StellarType.Planet) {
				w.WriteStartElement("planet");
			}

			foreach (Measurement entry in names) {
				w.WriteStartElement("name");
				w.WriteString(entry.MeasurementValue);
				w.WriteEndElement();
			}

			foreach (Measurement entry in measurements) {
				w.WriteStartElement(entry.MeasurementName);

				foreach (string key in entry.MeasurementAttributes.Keys) {
					w.WriteAttributeString (key, entry.MeasurementAttributes [key]);
				}

				w.WriteString (entry.MeasurementValue);

				w.WriteEndElement();
			}

			foreach(StellarObject entry in children){
				entry.Write (w);
			}

			w.WriteEndElement();
		}
    }
}
