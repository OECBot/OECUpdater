using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using OECLib.Data;

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
			if (node == null) {
				node = doc.FirstChild;
				root = new SolarSystem ();
				//Console.WriteLine ("firstChild = " + node.Name);
			}
			foreach (XmlNode child in node.ChildNodes) {
				//Console.WriteLine ("node: " + node.Name + "\tchild: " + child.Name + "\tvalue: " + child.InnerText);
				switch (child.Name) {
				case "binary":
					{
						Binary binary = new Binary ();
						root.AddChild (ParseXML (child, binary));
						break;
					}
				case "star":
					{
						Star star = new Star ();
						root.AddChild (ParseXML (child, star));
						break;
					}
				case "planet":
					{
						Planet planet = new Planet ();
						root.AddChild(ParseXML(child, planet));
						break;
					}
				default:
					{
						Measurement measure = CreateMeasurement (child);
						//Console.WriteLine ("Addind measurement: " + measure.MeasurementName);
						root.AddMeasurement (measure);
						break;
					}
				}
			}
			return root;
		}


		Measurement CreateMeasurement(XmlNode node) {
			Dictionary<string, string> attributes = new Dictionary<string, string> ();

			foreach (XmlAttribute xmlAttr in node.Attributes) {
				attributes.Add (xmlAttr.Name, xmlAttr.InnerText);
			}

			return new Measurement (node.Name, node.InnerText, attributes);
		}
	}
		
}

