using System;
using System.Collections.Generic;
using System.Xml;
using OECLib.Data.Measurements;

namespace OECLib.Data
{
    public abstract class StellarObject
    {
        public List<StellarObject> children;
		public List<StringMeasurement> names;
       	public Dictionary<string, Measurement> measurements;

        public StellarObject()
        {
            IsABinary = false;
            IsAPlanet = false;
            IsAStar = false;
            IsASystem = false;
            children = new List<StellarObject>();
			names = new List<StringMeasurement> ();
			measurements = new Dictionary<string, Measurement>();
        }

        public abstract bool AddChild(StellarObject child);

        public abstract XmlElement XMLTag(XmlDocument root);

        protected void AddMeasurementTags(XmlElement node, XmlDocument root)
        {
            foreach (Measurement name in names)
            {
                XmlElement element = root.CreateElement(name.MeasurementName);
                element = name.WriteXmlTag(element);
                node.AppendChild(element);
            }

            foreach (Measurement measurement in measurements.Values)
            {
                XmlElement element = root.CreateElement(measurement.MeasurementName);
                element = measurement.WriteXmlTag(element);
                node.AppendChild(element);
            }
        }
        
        public void AddStringMeasurement(string name, string measurement)
		{
			StringMeasurement strmeasurement = new StringMeasurement (name, measurement);
				
			if (name == "name") {
				if (!names.Contains (strmeasurement))
					names.Add(strmeasurement);
			} else { 
				if (!measurements.ContainsKey (name))
					measurements.Add (name, strmeasurement);
				else
					measurements [name] = strmeasurement;
			}
        }

        public void AddNumberMeasurement(string name, double measurement)
		{
			NumberMeasurement numMeasure = new NumberMeasurement (name, measurement);
				
			if (!measurements.ContainsKey (name))
				measurements.Add (name, numMeasure);
			else
				measurements [name] = numMeasure;
        }

        public void AddNumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus)
        {			
			NumberErrorMeasurement numErrMeasure = new NumberErrorMeasurement(name, measurement, errPlus, errMinus);	

			if (!measurements.ContainsKey (name))
				measurements.Add (name, numErrMeasure);
			else
				measurements [name] = numErrMeasure;
        }

		public void AddMeasurement(Measurement measurement)
		{
			if (measurement.MeasurementName == "name") {
				if(measurement.GetMeasurementType == MeasurementType.StringMeasurement) {
					names.Add((StringMeasurement)measurement);
				}
			} else {  
				measurements.Add (measurement.MeasurementName, measurement);
			}
		}



        public void ResetMeasurement()
        {
            this.measurements.Clear();
        }


        public bool IsABinary
        {
            get;
            protected set;
        }

        public bool IsAPlanet
        {
            get;
            protected set;
        }

        public bool IsAStar
        {
            get;
            protected set;
        }

        public bool IsASystem
        {
            get;
            protected set;
        }

		public void Write(XmlWriter w)
		{
			if (IsASystem) {
				w.WriteStartElement("system");
			}
			else if (IsAStar) {
				w.WriteStartElement("star");
			}
			else if (IsABinary) {
				w.WriteStartElement("binary");
			}
			else if (IsAPlanet) {
				w.WriteStartElement("planet");
			}

			foreach (Measurement entry in names) {
				w.WriteStartElement("name");
				w.WriteString((string) entry.getValue().value);
				w.WriteEndElement();
			}

			foreach (KeyValuePair<string, Measurement> entry in measurements) {
				w.WriteStartElement(entry.Value.MeasurementName);
				if(entry.Value.getValue().errorMinus != 0.0){
					w.WriteAttributeString("errorminus", entry.Value.getValue().errorMinus.ToString());
				}
				if(entry.Value.getValue().errorPlus != 0.0){
					w.WriteAttributeString("errorplus", entry.Value.getValue().errorPlus.ToString());
				}

				if (entry.Value.getValue ().value is double) {
					w.WriteString (((double)entry.Value.getValue ().value).ToString());

				} else {
					w.WriteString ((string)entry.Value.getValue ().value);
				}

				w.WriteEndElement();
			}

			foreach(StellarObject entry in children){
				entry.Write (w);
			}

			w.WriteEndElement();
		}
    }
}
