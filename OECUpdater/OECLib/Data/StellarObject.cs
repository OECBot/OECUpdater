using System;
using System.Collections.Generic;
using System.Xml;
using OECLib.Data.Measurements;

namespace OECLib.Data
{
    public abstract class StellarObject
    {
        protected List<StellarObject> children;
		 List<StringMeasurement> names;
        protected Dictionary<string, Measurement> measurements;

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
        
        public void AddStringMeasurement(string name, string measurement)
		{
			StringMeasurement strmeasurement = new StringMeasurement (name, measurement);
				
			if (name == "name") {
					names.Add(strmeasurement);
			} else {  
				measurements.Add (name, strmeasurement);
			}
        }

        public void AddNumberMeasurement(string name, double measurement)
        {
            measurements.Add(name, new NumberMeasurement(name, measurement));
        }

        public void AddNumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus)
        {
            measurements.Add(name, new NumberErrorMeasurement(name, measurement, errPlus, errMinus));
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
    }
}
