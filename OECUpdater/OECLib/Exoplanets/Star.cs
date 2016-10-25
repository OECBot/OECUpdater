using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OECLib.Exoplanets.Units;
using OECLib.Interface;
using System.Xml;

namespace OECLib.Exoplanets
{
    public class Star : XMLWritable
    {
        string[] order = { "name", "mass", "radius", "magV", "magB", "magJ", "magH",
            "magK", "temperature", "metallicity", "spectralType", "planet"};

        Dictionary<String, UnitError> elements;
        public List<Planet> planets { get; set; }


        public Star(List<Planet> planets)
        {
            this.elements = new Dictionary<string, UnitError>();
            this.planets = planets;
        }


        public void addElement(UnitError element)
        {
            elements.Add(element.name, element);
        }


        public void Write(XmlWriter w)
        {
            w.WriteStartElement("star");

            foreach (string element in order)
            {
                if (elements.ContainsKey(element))
                {
                    elements[element].Write(w);
                }
            }

            foreach (var planet in planets)
            {
                planet.Write(w);
            }

            w.WriteEndElement();
        }
    }
}
