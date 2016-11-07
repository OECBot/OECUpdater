﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Data
{
    public class Star : StellarObject
    {
        public Star() : base()
        {
            IsAStar = true;
        }

        public override bool AddChild(StellarObject child)
        {
            if(child.IsAPlanet)
            {
                children.Add(child);
                return true;
            }
            return false;
        }

        public override XmlElement XMLTag(XmlDocument root)
        {
            XmlElement currentElement = root.CreateElement("star");
            foreach (Measurements.Measurement measurement in measurements.Values)
            {
                XmlElement element = root.CreateElement(measurement.MeasurementName);
                element = measurement.WriteXmlTag(element);
                currentElement.AppendChild(element);
            }
            foreach (StellarObject child in children)
            {
                currentElement.AppendChild(child.XMLTag(root));
            }
            return currentElement;
        }
    }
}