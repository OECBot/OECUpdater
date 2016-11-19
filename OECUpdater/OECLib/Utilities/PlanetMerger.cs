using System;
using OECLib.Data;

namespace OECLib.Utilities
{
	public class PlanetMerger
	{

        public static StellarObject FindName(StellarObject mergeFrom, StellarObject mergeTo)
        {
            if (mergeFrom.GetType() == mergeTo.GetType())
            {
                foreach (var measure in mergeFrom.names)
                {
                    foreach (var toMeasure in mergeTo.names)
                    {
                        if (measure.MeasurementValue == toMeasure.MeasurementValue)
                            return mergeTo;
                    }
                }
            }

            foreach(StellarObject m in mergeTo.children)
            {
                var result = FindName(mergeFrom, m);
                if (result != null) return result;
            }
            return null;
        }

        public static bool Merge(StellarObject mergeFrom, StellarObject mergeTo)
        {

            StellarObject stellar = FindName(mergeFrom, mergeTo);

            if (stellar == null) return false;

			foreach(var m in mergeFrom.measurements)
			{
				mergeTo.AddMeasurement (m);
			}

			foreach (var child in mergeFrom.children) {
				int index;
				if ((index=stellar.children.IndexOf (child)) != -1) {
					foreach(var m in child.measurements)
					{
						stellar.children [index].AddMeasurement (m);
					}
				} else
					stellar.AddChild (child);
			}

            return true;
        }

		public static void TestMerge() {

			XMLDeserializer sys1Desrializer = new XMLDeserializer ("16 Cygni.xml", true);
			StellarObject sys1 = sys1Desrializer.ParseXML ();

			Star star1 = new Star ();
			star1.AddMeasurement ("name", "Gliese 765.1 B");

			Planet planet1 = new Planet ();
			planet1.AddMeasurement ("name", "16 Cygni B b");
			planet1.AddMeasurement ("list", "weird planets");
			planet1.AddMeasurement ("description", "ayy lmao");
			planet1.AddMeasurement ("discoverymethod", "IDK");

			star1.AddChild (planet1);

			Console.WriteLine (sys1.XMLTag (new System.Xml.XmlDocument ()).OuterXml);

			Merge (star1, sys1);
			Console.Write ('\n');
			Console.WriteLine(sys1.XMLTag(new System.Xml.XmlDocument()).OuterXml);
		}
	}
}

