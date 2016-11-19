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

        public override bool AddChild(StellarObject child)
        {
			StellarType childType = child.ObjectType;
			if((numOfChildren == 0 || childType == StellarType.Planet) && childType != StellarType.System)
            {
                children.Add(child);
				if(childType != StellarType.Planet)
                    numOfChildren++;
                return true;
            }
            return false;
        }

		public override bool RemoveChild (StellarObject child)
		{
			return false;
		}

		public override StellarType ObjectType {
			get {
				return StellarType.System;
			}
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
