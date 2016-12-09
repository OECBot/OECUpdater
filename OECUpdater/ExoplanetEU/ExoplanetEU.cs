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
			Dictionary<string, Star> starList;
			HTTPRequest req = new HTTPRequest();
            
            string filter = "updated >= \"" + date + "\"";
            if (sysName != null)
            {
                filter += String.Format(" and \"{0}\" in name", sysName);
            }
            string data = req.RequestAsString(String.Format(baseURL, filter), null);

			starList = updateStars (data);
            StringReader sr = new StringReader(data);
			string line = sr.ReadLine();
            
			while ((line = sr.ReadLine()) != null) {
                String[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				Planet planet = new Planet ();
				Star star;

				string starName = fields[65];
				Logger.WriteLine (starName);
				if (starList.ContainsKey(starName))
					star = starList [starName];
				else
					continue;

                List<string> names = new List<String>();
				planet.AddMeasurement("name", fields[0]);
                string[] altNames = fields[63].Split(',');

                foreach (string name in altNames)
				{
					planet.AddMeasurement("name", name);
                }

				star.Source = string.Format("http://exoplanet.eu/catalog/{0}/", fields[1].Replace(' ', '+'));

				string detectionMethod = fields [60].ToLower();
				bool transit = false;
				bool rv = false;

				if (detectionMethod == "transit") {
					transit = true;
				} else if (detectionMethod == "radial velocity") {
					detectionMethod = "RV";
					rv = true;
				}

				//there might be more, but i'm not sure if they are the same

				planet.AddMeasurement("semimajoraxis", fields[13], new Dictionary<string, string> {
					{ "errorplus", fields[14]},
					{ "errorminus", fields[15]},
				});
				planet.AddMeasurement("eccentricity", fields[16], new Dictionary<string, string> {
					{ "errorplus", fields[17]},
					{ "errorminus", fields[18]},
				});

				planet.AddMeasurement("separation", fields[22], new Dictionary<string, string> {
					{ "errorplus", fields[23]},
					{ "errorminus", fields[24]},
				});

				planet.AddMeasurement("periastron", fields[25], new Dictionary<string, string> {
					{ "errorplus", fields[26]},
					{ "errorminus", fields[27]},
				});

				planet.AddMeasurement("longitiude", fields[11]);
				planet.AddMeasurement("inclination", fields[19], new Dictionary<string, string> {
					{ "errorplus", fields[20]},
					{ "errorminus", fields[21]},
				});
				planet.AddMeasurement("impactparamater", fields[43], new Dictionary<string, string> {
					{ "errorplus", fields[44]},
					{ "errorminus", fields[45]},
				});
				planet.AddMeasurement("period", fields[10], new Dictionary<string, string> {
					{ "errorplus", fields[11]},
					{ "errorminus", fields[12]},
				});
					
				planet.AddMeasurement("periastrontime", fields[28], new Dictionary<string, string> {
					{ "errorplus", fields[29]},
					{ "errorminus", fields[30]},
				});

				if (rv) {
					planet.AddMeasurement ("mass", fields [4], new Dictionary<string, string> {
						{ "errorplus", fields [5] },
						{ "errorminus", fields [6] },
						{ "type", "msini" }
					});
				} else {
					planet.AddMeasurement ("mass", fields [1], new Dictionary<string, string> {
						{ "errorplus", fields [2] },
						{ "errorminus", fields [3] },
					});
				}

				planet.AddMeasurement("radius", fields[7], new Dictionary<string, string> {
					{ "errorplus", fields[8]},
					{ "errorminus", fields[9]},
				});
				planet.AddMeasurement("temperature", fields[34]);

				/*if (transit) {
					planet.AddMeasurement ("istransitting", fields [21]);
					planet.AddMeasurement ("transittime", fields [22], new Dictionary<string, string> {
						{ "errorplus", fields [23] },
						{ "errorminus", fields [24] },
					});
				}*/


				planet.AddMeasurement("spinorbitalalignment", fields[40], new Dictionary<string, string> {
					{ "errorplus", fields[41]},
					{ "errorminus", fields[42]},
				});

				planet.AddMeasurement("detectionMethod", detectionMethod);
				string[] time = fields[24].Split('-');
				planet.AddMeasurement("lastupdate", (string.Format("{0}/{1}/{2}", time[0].Substring(2, 2), time[1], time[2])));
				planet.AddMeasurement("discovery", fields[23]);
                star.AddChild(planet);
				planets.Add (star);
            }
            return planets;
		}

		Dictionary<string, Star> updateStars(string httpResponse) {
			string line;
			StringReader sr = new StringReader(httpResponse);
			sr.ReadLine();
			Dictionary<string, Star> starList = new Dictionary<string, Star> ();

			while ((line = sr.ReadLine ()) != null) {
				string[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				string starName = fields [65];

				if(starList.ContainsKey(starName))
					continue;

				Star star = new Star ();
				star.AddMeasurement("name", starName);
				string[] altNames = fields[94].Split(',');

				foreach (string name in altNames)
				{
					star.AddMeasurement("name", name);
				}

				star.AddMeasurement("mass", fields[79], new Dictionary<string, string> {
					{ "errorplus", fields[80]},
					{ "errorminus", fields[81]},
				});
				star.AddMeasurement("radius", fields[82], new Dictionary<string, string> {
					{ "errorplus", fields[83]},
					{ "errorminus", fields[84]},
				});
				star.AddMeasurement("temperature", fields[89], new Dictionary<string, string> {
					{ "errorplus", fields[90]},
					{ "errorminus", fields[91]},
				});
				star.AddMeasurement ("age", fields[86], new Dictionary<string, string> {
					{ "errorplus", fields[87]},
					{ "errorminus", fields[88]},
				});
				star.AddMeasurement ("metallicity", fields[76]);
				star.AddMeasurement ("spectraltype", fields[85]);

				//star.AddMeasurement ("magB", fields[15]);
				star.AddMeasurement ("magV", fields[68]);
				//star.AddMeasurement ("magR", fields[17]);
				star.AddMeasurement ("magI", fields[69]);
				star.AddMeasurement ("magJ", fields[70]);
				star.AddMeasurement ("magH", fields[71]);
				star.AddMeasurement ("magK", fields[71]);

				starList.Add (starName, star);
			}
			return starList;
		}
	}
}
