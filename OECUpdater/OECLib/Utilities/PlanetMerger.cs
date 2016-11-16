using System;
using OECLib.Data;

namespace OECLib.Utilities
{
	public class PlanetMerger
	{

        public static StellarObject FindName(StellarObject mergeFrom, StellarObject mergeTo)
        {
			foreach (var measure in mergeFrom.names) {
				string name = measure.MeasurementValue;
				if (mergeTo.names.Contains (measure))
					return mergeTo;
			}
            foreach(StellarObject m in mergeTo.children)
            {
                var result = FindName(mergeFrom, m);
                if (result != null) return result;
            }
            return null;
        }
        public static StellarObject Merge(StellarObject mergeFrom, StellarObject mergeTo)
        {

            StellarObject stellar = FindName(mergeFrom, mergeTo);

            if (stellar == null) return mergeFrom;

            foreach(var m in mergeFrom.measurements)
            {
				mergeTo.AddMeasurement (m);
			}

			foreach (var child in mergeFrom.children) {
				stellar = Merge (child, stellar);
			}

            return mergeTo;
        }

		public static void TestMerge() {
			SolarSystem mergeTo = new SolarSystem ();
			Star testStar1 = new Star ();

			Planet testPlanet1 = new Planet ();
			Planet testPlanet2 = new Planet ();
			Planet testPlanet3 = new Planet ();

			testStar1.AddMeasurement ("name", "star1");

			testPlanet1.AddMeasurement ("name", "planet1");
			testPlanet2.AddMeasurement ("name", "planet2");
			testPlanet3.AddMeasurement ("name", "planet3");

			testPlanet1.AddMeasurement ("mass", "3.22");
			testPlanet2.AddMeasurement ("mass", "5.4");
			testPlanet3.AddMeasurement ("mass", "4.22");

			testStar1.AddChild (testPlanet1);
			testStar1.AddChild (testPlanet2);
			mergeTo.AddChild (testStar1);

			testStar1.AddChild (testPlanet3);
			Console.WriteLine (mergeTo.XMLTag (new System.Xml.XmlDocument ()));
			Console.WriteLine(Merge (testStar1, mergeTo).XMLTag(new System.Xml.XmlDocument()));
		}
	}
}

