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
        public List<String> names { get; set; }
        public UnitError mass { get; set; }
        public UnitError radius { get; set; }
        public float magV { get; set; }
        public UnitError magB { get; set; }
        public UnitError magJ { get; set; }
        public UnitError magH { get; set; }
        public UnitError magK { get; set; }
        public UnitError temperature { get; set; }
        public UnitError metallicity { get; set; }
        public String spectralType { get; set; }
        public List<Planet> planets { get; set; }

        public Star(List<String> names, UnitError mass, UnitError radius, float magV,
            UnitError magB, UnitError magJ, UnitError magH, UnitError magK, UnitError temperature,
            UnitError metallicity, String spectralType, List<Planet> planets)
        {
            this.names = names;
            this.mass = mass;
            this.radius = radius;
            this.magV = magV;
            this.magB = magB;
            this.magJ = magJ;
            this.magH = magH;
            this.magK = magK;
            this.temperature = temperature;
            this.metallicity = metallicity;
            this.spectralType = spectralType;
            this.planets = planets;
        }

        public void Write(XmlWriter w)
        {
            w.WriteStartElement("star");

            foreach (var name in names)
            {
                w.WriteElementString("name", name);
            }
            mass.Write(w);
            radius.Write(w);
            w.WriteElementString("magV", magV.ToString());
            magB.Write(w);
            magJ.Write(w);
            magH.Write(w);
            magK.Write(w);
            temperature.Write(w);
            metallicity.Write(w);
            w.WriteElementString("spectraltype", spectralType);

            foreach (var planet in planets)
            {
                planet.Write(w);
            }

            w.WriteEndElement();
        }
    }
}
