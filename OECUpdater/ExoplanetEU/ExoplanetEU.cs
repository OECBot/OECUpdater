using System;
using System.Xml;

using OECLib.Interface;
using OECLib.Data;
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

		public List<StellarObject> Run()
		{
			List<StellarObject> planets = new List<StellarObject>();
            HTTPRequest req = new HTTPRequest();
            string date = "2016-09-01";
            string filter = "updated >= \""+date+"\"";
            string data = req.RequestAsString(String.Format(baseURL, filter), null);
            StringReader sr = new StringReader(data);
            sr.ReadLine();
            String line = null;
            while ((line = sr.ReadLine()) != null) {

                String[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				Planet planet = new Planet ();

                List<string> names = new List<String>();
				planet.AddMeasurement("name", fields[0]);
                string[] altNames = fields[63].Split(',');

                foreach (string name in altNames)
				{
					planet.AddMeasurement("name", name);
                }

				planet.AddMeasurement("source", string.Format("http://exoplanet.eu/catalog/{0}/", fields[0].Replace(" ", "_")));

				planet.AddMeasurement("mass", fields[1], new Dictionary<string, string> {
					{ "errorplus", fields[2]},
					{ "errorminus", fields[3]},
				});
				planet.AddMeasurement("period", fields[10], new Dictionary<string, string> {
					{ "errorplus", fields[11]},
					{ "errorminus", fields[12]},
				});
				planet.AddMeasurement("semimajoraxis", fields[13], new Dictionary<string, string> {
					{ "errorplus", fields[14]},
					{ "errorminus", fields[15]},
				});
				planet.AddMeasurement("eccentricity", fields[16], new Dictionary<string, string> {
					{ "errorplus", fields[17]},
					{ "errorminus", fields[18]},
				});
				planet.AddMeasurement("periastron", fields[25], new Dictionary<string, string> {
					{ "errorplus", fields[26]},
					{ "errorminus", fields[27]},
				});
				planet.AddMeasurement("periastrontime", fields[28], new Dictionary<string, string> {
					{ "errorplus", fields[29]},
					{ "errorminus", fields[30]},
				});

				planet.AddMeasurement("detectionMethod", fields[60] == "Radial Velocity" ? "RV" : "IDK");
				string[] time = fields[24].Split('-');
				planet.AddMeasurement("lastUpdate", (string.Format("{0}/{1}/{2}", time[0].Substring(2, 2), time[1], time[2])));
				planet.AddMeasurement("discovery", fields[23]);
				planets.Add(planet);

            }
            return planets;
		}
	}
}