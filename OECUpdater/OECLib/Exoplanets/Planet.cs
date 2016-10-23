using OECLib.Exoplanets.Units;
using OECLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OECLib.Exoplanets
{
    public class Planet : XMLWritable
    {
        public List<String> names { get; set; }
        public UnitError mass { get; set; }
        public UnitError period { get; set; }
        public UnitError semiMajorAxis { get; set; }
        public UnitError eccentricity { get; set; }
        public UnitError periastron { get; set; }
        public UnitError periastronTime { get; set; }
        public String description { get; set; }
        public String discoveryMethod { get; set; }
        public String lastUpdate { get; set; }
        public String discoveryYear { get; set; }

        public Planet(List<String> names, UnitError mass, UnitError period, UnitError semimajoraxis,
            UnitError eccentricity, UnitError periastron, UnitError periastronTime, String description,
            String discoverymethod, String lastupdate, String discoveryYear)
        {
            this.names = names;
            this.mass = mass;
            this.period = period;
            this.semiMajorAxis = semimajoraxis;
            this.eccentricity = eccentricity;
            this.periastron = periastron;
            this.periastronTime = periastronTime;
            this.description = description;
            this.discoveryMethod = discoverymethod;
            this.lastUpdate = lastupdate;
            this.discoveryYear = discoveryYear;
        }


        public void Write(XmlWriter w)
        {
            w.WriteStartElement("planet");

            foreach (var name in names)
            {
                w.WriteElementString("name", name);
            }

            mass.Write(w);
            period.Write(w);
            semiMajorAxis.Write(w);
            eccentricity.Write(w);
            periastron.Write(w);
            periastronTime.Write(w);
            w.WriteElementString("description", description);
            w.WriteElementString("discovermethod", discoveryMethod);
            w.WriteElementString("lastupdate", lastUpdate);
            w.WriteElementString("discoveryear", discoveryYear);

            w.WriteEndElement();
        }
    }
}
