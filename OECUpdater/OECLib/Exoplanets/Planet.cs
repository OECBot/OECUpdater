using OECLib.Exoplanets.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECLib.Exoplanets
{
    public class Planet
    {
        public List<String> names { get; set; }
        public UnitError mass { get; set; }
        public UnitError period { get; set; }
        public UnitError semiMajorAxis { get; set; }
        public UnitError eccentricity { get; set; }
        public UnitError periastron { get; set; }
        public UnitError periastronTime { get; set; }
        public String discoveryMethod { get; set; }
        public String lastUpdate { get; set; }
        public String discovery { get; set; }

        public Planet(List<String> names, UnitError mass, UnitError period, UnitError semimajoraxis, UnitError eccentricity,
            UnitError periastron, UnitError periastrontime, String discoverymethod, String lastupdate, String discovery)
        {
            this.names = names;
            this.mass = mass;
            this.period = period;
            this.semiMajorAxis = semimajoraxis;
            this.eccentricity = eccentricity;
            this.periastron = periastron;
            this.periastronTime = periastronTime;
            this.discoveryMethod = discoverymethod;
            this.lastUpdate = lastupdate;
            this.discovery = discovery;
        }

        public String toString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", names, mass, period, semiMajorAxis, eccentricity, periastron, periastronTime, discoveryMethod, lastUpdate, discovery);
        }
    }
}
