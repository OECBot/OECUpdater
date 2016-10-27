using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OECLib.Exoplanets.Units;
using OECLib.Interface;
using System.Xml;
using System.Collections;

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
            if (elements.ContainsKey(element.name)){
                // Check to see if there is already a list created with the new elements name
                if (elements[element.name].value is IList<object>)
                {
                    IList collection = (IList)elements[element.name].value;
                    collection.Add(element.value);
                }
                // Create a list if there are multiple elements which have the same name.
                // For example a star with multiple names. 
                else
                {
                    List<object> allElements = new List<object>();
                    allElements.Add(elements[element.name].value);
                    allElements.Add(element.value);
                    elements[element.name].value = allElements;
                }
            }
            else
            {
                elements.Add(element.name, element);
            }
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
