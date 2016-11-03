using System;
using OECLib.Utilities;
using OECLib.Data;

namespace OECUpdater
{
	public class DeserializeTest
	{
		public DeserializeTest (string filename)
		{
			XMLDeserializer xml = new XMLDeserializer (filename);
			StellarObject obj = xml.ParseXML ();
			Console.Write(obj.XMLTag(new System.Xml.XmlDocument()).OuterXml);
		}
	}
}

