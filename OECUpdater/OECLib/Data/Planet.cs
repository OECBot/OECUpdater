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
        public override bool AddChild(StellarObject child)
        {
            return false;
        }

		public override bool RemoveChild(StellarObject child) {
			return false;
		}

		public override StellarType ObjectType {
			get {
				return StellarType.Planet;
			}
		}

        public override XmlElement XMLTag(XmlDocument root)
        {
            XmlElement currentElement = root.CreateElement("planet");
            AddMeasurementTags(currentElement, root);
            return currentElement;
        }
    }
}
