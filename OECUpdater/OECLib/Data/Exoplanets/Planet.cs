﻿using OECLib.Exoplanets.Units;
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
        public string[] order = { "name", "mass", "period", "semimajoraxis", "eccentricity",
            "periastron", "periastrontime", "description", "discovermethod", "lastupdate",
            "discoveryear"};
        public Dictionary<String, UnitError> elements;
        public String source;

        public Planet()
        {
            this.elements = new Dictionary<string, UnitError>();
        }

        public void setSource(String source)
        {
            this.source = source;
        }

        public void addElement(UnitError element)
        {
            elements.Add(element.name, element);
        }


        public void Write(XmlWriter w)
        {
            w.WriteStartElement("planet");

            foreach (string element in order)
            {
                if (elements.ContainsKey(element))
                {
                    elements[element].Write(w);
                }
            }

            w.WriteEndElement();
        }
    }
}