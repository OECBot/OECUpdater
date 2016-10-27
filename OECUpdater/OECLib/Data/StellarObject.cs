using System;
using System.Collections.Generic;
using System.Xml;
using OECLib.Data.Measurements;

namespace OECLib.Data
{
    public abstract class StellarObject
    {
        protected List<StellarObject> children;
        protected Dictionary<string, Measurement> measurements;

        public StellarObject()
        {
            IsABinary = false;
            IsAPlanet = false;
            IsAStar = false;
            IsASystem = false;
            children = new List<StellarObject>();
            measurements = new Dictionary<string, Measurement>();
        }

        public abstract bool AddChild(StellarObject child);

        public abstract XmlElement XMLTag(XmlDocument root);
        
        public void AddStringMeasurement(string name, string measurement)
        {
            measurements.Add(name, new StringMeasurement(name, measurement));
        }

        public void AddNumberMeasurement(string name, double measurement)
        {
            measurements.Add(name, new NumberMeasurement(name, measurement));
        }

        public void AddNumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus)
        {
            measurements.Add(name, new NumberErrorMeasurement(name, measurement, errPlus, errMinus));
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
