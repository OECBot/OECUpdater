using System;
using OECLib.Utilities;
using OECLib.Data;
using System.Collections.Generic;
using OECLib.Data.Measurements;

namespace OECUpdater
{
	public class DeserializeTest
	{
		public DeserializeTest (string filename)
		{
			XMLDeserializer xml = new XMLDeserializer (filename, true);
			StellarObject obj = xml.ParseXML ();
			printMeasurements (obj);
			Serializer.writeToXML ("test2.xml",(SolarSystem)obj);
		}

		private void printMeasurements(StellarObject obj){
			Console.WriteLine ("NEW STELLAR OBJECT");

			foreach (Measurement entry in obj.names) {
				Console.WriteLine ("Name: " + entry.getValue ().value);
			}

			foreach (KeyValuePair<string, Measurement> entry in obj.measurements) {
				if (entry.Value.getValue().value is double) {
					Console.WriteLine ("MeasureName: " + entry.Value.MeasurementName
						+ "\tdouble: " + entry.Value.getValue().value);
				} else {
					Console.WriteLine ("MeasureName: " + entry.Value.MeasurementName 
						+ "\tNOT double: " + entry.Value.getValue().value);
				}
			}

			foreach (StellarObject entry in obj.children) {
				printMeasurements (entry);
			}
		}
	}
}

