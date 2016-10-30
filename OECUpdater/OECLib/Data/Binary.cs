using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data
{

    public class Binary : StellarObject
    {
        int numberOfBinaries = 0;
        int numberOfStars = 0;

        public Binary() : base()
        {
            IsABinary = true;
        }

        public override bool AddChild(StellarObject child)
        {
            bool canAdd = (numberOfBinaries == 1 && numberOfStars == 0) || (numberOfBinaries == 0 && numberOfStars == 1) || (numberOfStars == 0 && numberOfStars == 0) ;
            bool successfulAdd = false;

            if(canAdd || child.IsAPlanet)
            {
                children.Add(child);

                if (child.IsABinary)
                    numberOfBinaries++;
                else if (child.IsAStar)
                    numberOfStars++;

                successfulAdd = true;
            }
            return successfulAdd;
        }

        public override XmlElement XMLTag(XmlDocument root)
        {
             XmlElement currentElement = root.CreateElement("binary");
             foreach(Measurements.Measurement measurement in measurements.Values)
             {
                 XmlElement element = root.CreateElement(measurement.MeasurementName);
                 element = measurement.WriteXmlTag(element);
                 currentElement.AppendChild(element);
             }
             foreach(StellarObject child in children)
             {
                 currentElement.AppendChild(child.XMLTag(root));
             }
             return currentElement;
        }
    }
}
