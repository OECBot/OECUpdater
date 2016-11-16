using System;
using System.Xml;

using OECLib.Interface;
using OECLib.Data;
using OECLib.Data.Measurements;
using System.Collections.Generic;
using OECLib.Utilities;
using System.IO;
using System.Text.RegularExpressions;

namespace ExoplanetEU
{
	public class ExoplanetEU : IPlugin
	{

        private String baseURL = "http://exoplanet.eu/catalog/csv/?status=&f={0}";

		public void Initialize()
		{

		}

		public String GetName()
		{
			return "Exoplanet.EU";
		}

		public String GetDescription()
		{
			return "This plugin allows the extraction of data from the exoplanet.eu database";
		}

		public String GetAuthor()
		{
			return "Spazio";
		}

		public List<StellarObject> Run(String date, String sysName=null)
		{
			List<StellarObject> planets = new List<StellarObject>();
            HTTPRequest req = new HTTPRequest();
            //string date = "2016-09-01";
            string filter = "updated >= \""+date+"\"";
            if (sysName != null)
            {
                filter += String.Format("and \"{0}\" in name", sysName);
            }
            string data = req.RequestAsString(String.Format(baseURL, filter), null);
            StringReader sr = new StringReader(data);
            sr.ReadLine();
            String line = null;
            while ((line = sr.ReadLine()) != null) {

                String[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				Planet planet = new Planet ();

                List<string> names = new List<String>();
				planet.AddStringMeasurement("name", fields[0]);
                string[] altNames = fields[63].Split(',');

                foreach (string name in altNames)
				{
					planet.AddStringMeasurement("name", name);
                }

				planet.AddStringMeasurement("source", string.Format("http://exoplanet.eu/catalog/{0}/", fields[0].Replace(" ", "_")));
				planet.AddNumberErrorMeasurement("mass", parseDouble(fields[1]), parseDouble(fields[2]), parseDouble(fields[3]));
				planet.AddNumberErrorMeasurement("period", parseDouble(fields[10]), parseDouble(fields[11]), parseDouble(fields[12]));
				planet.AddNumberErrorMeasurement("semimajoraxis", parseDouble(fields[13]), parseDouble(fields[14]), parseDouble(fields[15]));
				planet.AddNumberErrorMeasurement("eccentricity", parseDouble(fields[16]), parseDouble(fields[17]), parseDouble(fields[18]));
				planet.AddNumberErrorMeasurement("periastron", parseDouble(fields[25]), parseDouble(fields[26]), parseDouble(fields[27]));
				planet.AddNumberErrorMeasurement("periastrontime", parseDouble(fields[28]), parseDouble(fields[29]), parseDouble(fields[29]));
				planet.AddStringMeasurement("detectionMethod", fields[60] == "Radial Velocity" ? "RV" : "IDK");
				string[] time = fields[24].Split('-');
				planet.AddStringMeasurement("lastUpdate", (string.Format("{0}/{1}/{2}", time[0].Substring(2, 2), time[1], time[2])));
				planet.AddStringMeasurement("discovery", fields[23]);

						planets.Add(planet);

            }
            return planets;
		}

        public StellarObject GetSystem(String name)
        {
            String filter = String.Format("\"{0}\" in name", name);
            HTTPRequest req = new HTTPRequest();
            String data = req.RequestAsString(String.Format(baseURL, filter), null);
            StringReader sr = new StringReader(data);
            sr.ReadLine();
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {

            }

            return null;
        }

        private Double parseDouble(String value)
        {
            return value == "" ? 0.0 : Double.Parse(value);
        }
	}
}

