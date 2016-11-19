using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Data
{
    public class Star : StellarObject
    {
        public override bool AddChild(StellarObject child)
        {
			if(child.ObjectType == StellarType.Planet)
            {
                children.Add(child);
                return true;
            }
            return false;
        }

		public override bool RemoveChild(StellarObject child) {
			return children.Remove (child);
		}

		public override StellarType ObjectType {
			get {
				return StellarType.Star;
			}
		}

        public override XmlElement XMLTag(XmlDocument root)
        {
            XmlElement currentElement = root.CreateElement("star");

            AddMeasurementTags(currentElement, root);

            foreach (StellarObject child in children)
            {
                currentElement.AppendChild(child.XMLTag(root));
            }
            return currentElement;
        }
    }
}
