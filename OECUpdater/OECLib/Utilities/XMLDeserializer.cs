using System;
using System.Xml;
using System.IO;
using OECLib.Data;
using OECLib.Data.Measurements;

namespace OECLib.Utilities
{
	public class XMLDeserializer
	{
		XmlDocument doc;

		public XMLDeserializer (string filename)
		{
			doc = new XmlDocument ();
			doc.LoadXml (File.ReadAllText (filename));
		}

		public StellarObject ParseXML(XmlNode node=null, StellarObject root=null) {
            if (node == null)
            {
                node = doc.FirstChild;
                SolarSystem system = new SolarSystem();
                system.AddChild(ParseXML(node, system));
                return system;
            }
            else
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "binary":
                            {
                                Binary binary = new Binary();
                                root.AddChild(ParseXML(child, binary));
                                break;
                            }
                        case "star":
                            {
                                Star star = new Star();
                                root.AddChild(ParseXML(child, star));
                                break;
                            }
                        case "planet":
                            {
                                Planet planet = new Planet();
                                root.AddChild(ParseXML(child, planet));
                                break;
                            }
                        default:
                            root.AddMeasurement(CreateMeasurement(child));
                            break;
                    }
                }
            }
			return root;
		}


		Measurement CreateMeasurement(XmlNode node) {
			Measurement measurement;
			string name = node.Name;

            if (node.Attributes != null && node.Attributes["errorminus"] != null) {
				double errminus = parseDouble (node.Attributes.GetNamedItem ("errorminus").InnerText);
				double errplus = parseDouble (node.Attributes.GetNamedItem ("errorplus").InnerText);
				double nummeasurement = parseDouble (node.InnerText);

				measurement = new NumberErrorMeasurement (name, nummeasurement, errplus, errminus);
			} else {
				double nummeasurement = parseDouble (node.InnerText);
				if (double.IsNaN (nummeasurement)) {
					string strmeasurement = node.InnerText;
					measurement = new StringMeasurement (name, strmeasurement);
				} else {
					measurement = new NumberMeasurement (name, nummeasurement);
				}
			}
				
			return measurement;
		}

		double parseDouble(string value) {
			double result;
			if (double.TryParse (value, out result)) {
				return result;
			}
			return double.NaN;
		}
	}
		
}

