using System;
using System.Xml;

using OECLib.Plugins;
using OECLib.Exoplanets;
using System.Collections.Generic;
using OECLib.HTTPRequests;
using System.IO;
using System.Text.RegularExpressions;
using OECLib.Exoplanets.Units;

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

		public List<Planet> Run()
		{
            List<Planet> planets = new List<Planet>();
            HTTPRequest req = new HTTPRequest();
            String date = "2016-09-01";
            String filter = "updated >= \""+date+"\"";
            String data = req.RequestAsString(String.Format(baseURL, filter), null);
            StringReader sr = new StringReader(data);
            Console.WriteLine(sr.ReadLine());
            String line = null;
            while ((line = sr.ReadLine()) != null) {

                String[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                List<String> names = new List<String>();
                names.Add(fields[0]);
                String[] altNames = fields[63].Split(',');
                for (int i = 0; i < altNames.Length; i++)
                {
                    names.Add(altNames[i]);
                }

                UnitError mass = new UnitError("mass", (fields[1]), (fields[2]), (fields[3]));
                UnitError period = new UnitError("period", (fields[10]), (fields[11]), (fields[12]));
                UnitError semiMajorAxis = new UnitError("semimajoraxis", (fields[13]), (fields[14]), (fields[15]));
                UnitError eccentricity = new UnitError("eccentricity", (fields[16]), (fields[17]), (fields[18]));
                UnitError periastron = new UnitError("periastron", (fields[25]), (fields[26]), (fields[27]));
                UnitError periastronTime = new UnitError("periastrontime", (fields[28]), (fields[29]), (fields[29]));
                String detectionMethod = fields[60] == "Radial Velocity" ? "RV" : "IDK";
                String[] time = fields[24].Split('-');
                String lastUpdate = String.Format("{0}/{1}/{2}", time[0].Substring(2, 2), time[1], time[2]);
                String discovery = fields[23];

                Planet newPlanet = new Planet(names, mass, period, semiMajorAxis, eccentricity, periastron, periastronTime, "", detectionMethod, lastUpdate, discovery);
                planets.Add(newPlanet);

            }
            return planets;
		}
	}
}

