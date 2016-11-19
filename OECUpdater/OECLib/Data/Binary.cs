using System;
using System.Collections.Generic;
using System.Xml;

namespace OECLib.Data
{

    public class Binary : StellarObject
    {
        int numberOfBinaries = 0;
        int numberOfStars = 0;

        public override bool AddChild(StellarObject child)
        {
            bool canAdd = (numberOfBinaries == 1 && numberOfStars == 0) || (numberOfBinaries == 0 && numberOfStars == 1) || (numberOfStars == 0 && numberOfStars == 0) ;
            bool successfulAdd = false;
			StellarType childType = child.ObjectType;

			if(canAdd || childType == StellarType.Planet)
            {
                children.Add(child);

				if (childType == StellarType.Binary)
                    numberOfBinaries++;
				else if (childType == StellarType.Star)
                    numberOfStars++;

                successfulAdd = true;
            }
            return successfulAdd;
        }

		public override bool RemoveChild(StellarObject child) {
			return false;
		}

		public override StellarType ObjectType {
			get {
				return StellarType.Binary;
			}
		}

        public override XmlElement XMLTag(XmlDocument root)
        {
            XmlElement currentElement = root.CreateElement("binary");
            AddMeasurementTags(currentElement, root);
            foreach (StellarObject child in children)
            {
                currentElement.AppendChild(child.XMLTag(root));
            }
            return currentElement;
        }
    }
}
