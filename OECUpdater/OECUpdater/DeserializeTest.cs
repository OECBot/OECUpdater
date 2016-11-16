using System;
using OECLib.Utilities;
using OECLib.Data;
using System.Collections.Generic;

namespace OECUpdater
{
	public class DeserializeTest
	{
		public DeserializeTest (string filename)
		{
			XMLDeserializer xml = new XMLDeserializer (filename);
			StellarObject obj = xml.ParseXML ();
			printMeasurements (obj);
			Serializer.writeToXML ("test2.xml",(SolarSystem)obj);
		}

		private void printMeasurements(StellarObject obj){
			Console.WriteLine ("NEW STELLAR OBJECT");

			foreach (Measurement entry in obj.names) {
				Console.WriteLine ("Name: " + entry.MeasurementValue);
			}

			foreach (Measurement entry in obj.measurements) {
				Console.WriteLine ("MeasureName: " + entry.MeasurementName 
					+ "\tMeasureValue: " + entry.MeasurementValue);
			}

			foreach (StellarObject entry in obj.children) {
				printMeasurements (entry);
			}
		}
	}
}

