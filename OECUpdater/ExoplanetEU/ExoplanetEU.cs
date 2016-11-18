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

        public List<StellarObject> Run(String date, String sysName = null)
        {
            List<StellarObject> planets = new List<StellarObject>();
            HTTPRequest req = new HTTPRequest();
            
            string filter = "updated >= \"" + date + "\"";
            if (sysName != null)
            {
                filter += String.Format(" and \"{0}\" in name", sysName);
            }
            string data = req.RequestAsString(String.Format(baseURL, filter), null);
            StringReader sr = new StringReader(data);
            sr.ReadLine();
            String line = null;
            while ((line = sr.ReadLine()) != null) {
                String[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                Star star = new Star();
				Planet planet = new Planet ();

                List<string> names = new List<String>();
				planet.AddMeasurement("name", fields[0]);
                string[] altNames = fields[63].Split(',');

                foreach (string name in altNames)
				{
					planet.AddMeasurement("name", name);
                }
                star.AddMeasurement("name", fields[65]);
                string[] starAltNames = fields[94].Split(',');
                foreach (string starName in starAltNames)
                {
                    planet.AddMeasurement("name", starName);
                }

                star.AddMeasurement("mass", fields[79], new Dictionary<string, string> {
                    { "errorplus", fields[81]},
                    { "errorminus", fields[80]},
                });

                star.AddMeasurement("radius", fields[82], new Dictionary<string, string> {
                    { "errorplus", fields[84]},
                    { "errorminus", fields[83]},
                });

                star.AddMeasurement("magV", fields[68]);
                star.AddMeasurement("magH", fields[70]);
                star.AddMeasurement("magJ", fields[71]);

                star.AddMeasurement("temperature", fields[89], new Dictionary<string, string> {
                    { "errorplus", fields[91]},
                    { "errorminus", fields[90]},
                });

                star.AddMeasurement("metallicity", fields[76], new Dictionary<string, string> {
                    { "errorplus", fields[78]},
                    { "errorminus", fields[77]},
                });

                star.AddMeasurement("spectraltype", fields[85]);

                star.AddMeasurement("age", fields[86], new Dictionary<string, string> {
                    { "errorplus", fields[88]},
                    { "errorminus", fields[87]},
                });

				planet.AddMeasurement("source", string.Format("http://exoplanet.eu/catalog/{0}/", fields[0].Replace(" ", "_")));

				planet.AddMeasurement("mass", fields[1], new Dictionary<string, string> {
					{ "errorplus", fields[3]},
					{ "errorminus", fields[2]},
				});
				planet.AddMeasurement("period", fields[10], new Dictionary<string, string> {
					{ "errorplus", fields[12]},
					{ "errorminus", fields[11]},
				});
				planet.AddMeasurement("semimajoraxis", fields[13], new Dictionary<string, string> {
					{ "errorplus", fields[15]},
					{ "errorminus", fields[14]},
				});
				planet.AddMeasurement("eccentricity", fields[16], new Dictionary<string, string> {
					{ "errorplus", fields[18]},
					{ "errorminus", fields[17]},
				});
				planet.AddMeasurement("periastron", fields[25], new Dictionary<string, string> {
					{ "errorplus", fields[27]},
					{ "errorminus", fields[26]},
				});
				planet.AddMeasurement("periastrontime", fields[28], new Dictionary<string, string> {
					{ "errorplus", fields[30]},
					{ "errorminus", fields[29]},
				});

				planet.AddMeasurement("detectionMethod", fields[60] == "Radial Velocity" ? "RV" : "IDK");
				string[] time = fields[24].Split('-');
				planet.AddMeasurement("lastUpdate", (string.Format("{0}/{1}/{2}", time[0].Substring(2, 2), time[1], time[2])));
				planet.AddMeasurement("discovery", fields[23]);
                star.AddChild(planet);
                planets.Add(star);
            }
            return planets;
		}
	}
}