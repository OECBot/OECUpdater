using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Data
{
    public class SolarSystem : StellarObject
    {
        int numOfChildren = 0;

        public SolarSystem() : base()
        {
            IsASystem = true;
        }

        public override bool AddChild(StellarObject child)
        {
            if(numOfChildren == 0 || child.IsAPlanet)
            {
                children.Add(child);
                if(!child.IsAPlanet)
                    numOfChildren++;
                return true;
            }
            return false;
        }

        public override XmlElement XMLTag(XmlDocument root)
        {
            XmlElement currentElement = root.CreateElement("system");

            AddMeasurementTags(currentElement, root);

            foreach (StellarObject child in children)
            {
                currentElement.AppendChild(child.XMLTag(root));
            }
            return currentElement;
        }
    }
}
