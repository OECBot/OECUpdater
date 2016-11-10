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

