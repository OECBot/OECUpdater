using OECLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using OECLib.Exoplanets;
using OECLib.Exoplanets.Units;


namespace OECLib.Exoplanets
{
    public class PlanetSystem : XMLWritable
    {
        string[] order = { "name", "rightascension", "declination", "distance"};
        public Dictionary<String, UnitError> elements;
        public List<Star> stars { get; set; }

        public PlanetSystem(List<Star> stars)
        {
            this.elements = new Dictionary<string, UnitError>();
            this.stars = stars;
        }


        public void addElement(UnitError element)
        {
            elements.Add(element.name, element);
        }


        public void Write(XmlWriter w)
        {
            w.WriteStartElement("system");

            foreach (string element in order)
            {
                if (elements.ContainsKey(element))
                {
                    elements[element].Write(w);
                }
            }

            foreach (var star in stars)
            {
                star.Write(w);
            }

            w.WriteEndElement();
        }

    }
}
