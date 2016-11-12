using System;
using OECLib.Data;
using OECLib.Data.Measurements;

namespace OECLib.Utilities
{
	public class PlanetMerger
	{

        public static StellarObject FindName(StellarObject mergeFrom, StellarObject mergeTo)
        {
			foreach (var measure in mergeFrom.names) {
				string name = (string)measure.getValue ().value;
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
            //mergeTo.ResetMeasurement();

			foreach (var child in mergeFrom.children) {
				stellar = Merge (child, stellar);
			}

            foreach(var m in mergeFrom.measurements)
            {
				mergeTo.AddMeasurement (m.Value);
            }
            return mergeTo;
        }

		public static void TestMerge() {
			SolarSystem mergeTo = new SolarSystem ();
			Star testStar1 = new Star ();

			Planet testPlanet1 = new Planet ();
			Planet testPlanet2 = new Planet ();
			Planet testPlanet3 = new Planet ();

			testStar1.AddStringMeasurement ("name", "star1");

			testPlanet1.AddStringMeasurement ("name", "planet1");
			testPlanet2.AddStringMeasurement ("name", "planet2");
			testPlanet3.AddStringMeasurement ("name", "planet3");

			testPlanet1.AddNumberMeasurement ("mass", 3.22);
			testPlanet2.AddNumberMeasurement ("mass", 5.4);
			testPlanet3.AddNumberMeasurement ("mass", 4.22);

			testStar1.AddChild (testPlanet1);
			testStar1.AddChild (testPlanet2);
			mergeTo.AddChild (testStar1);

			testStar1.AddChild (testPlanet3);
			Console.WriteLine (mergeTo.XMLTag (new System.Xml.XmlDocument ()));
			Console.WriteLine(Merge (testStar1, mergeTo).XMLTag(new System.Xml.XmlDocument()));
		}
	}
}

