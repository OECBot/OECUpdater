using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Data
{
    public class Planet : StellarObject
    {
        public Planet() : base()
        {
            IsAPlanet = true;
        }
        public override bool AddChild(StellarObject child)
        {
            return false;
        }

        public override XmlElement XMLTag(XmlDocument root)
        {
            XmlElement currentElement = root.CreateElement("planet");
            AddMeasurementTags(currentElement, root);
            return currentElement;
        }
    }
}
