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
        public String name { get; set; }
        public String rightAscension { get; set; }
        public String declination { get; set; }
        public UnitError distance { get; set; }
        public Star star { get; set; }

        public PlanetSystem(String name, String rightAscension, String declination,
            UnitError distance, Star star)
        {
            this.name = name;
            this.rightAscension = rightAscension;
            this.declination = declination;
            this.distance = distance;
            this.star = star;
        }

        public void Write(XmlWriter w)
        {
            w.WriteStartElement("system");
            w.WriteElementString("name", name);
            w.WriteElementString("rightascension", rightAscension);
            w.WriteElementString("declination", declination);
            distance.Write(w);
            star.Write(w);
            w.WriteEndElement();
        }

    }
}
