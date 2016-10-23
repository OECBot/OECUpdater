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

                foreach (String name in altNames)
                {
                    names.Add(name);
                }

                UnitError mass = new UnitError("mass", parseDouble(fields[1]), parseDouble(fields[2]), parseDouble(fields[3]));
                UnitError period = new UnitError("period", parseDouble(fields[10]), parseDouble(fields[11]), parseDouble(fields[12]));
                UnitError semiMajorAxis = new UnitError("semimajoraxis", parseDouble(fields[13]), parseDouble(fields[14]), parseDouble(fields[15]));
                UnitError eccentricity = new UnitError("eccentricity", parseDouble(fields[16]), parseDouble(fields[17]), parseDouble(fields[18]));
                UnitError periastron = new UnitError("periastron", parseDouble(fields[25]), parseDouble(fields[26]), parseDouble(fields[27]));
                UnitError periastronTime = new UnitError("periastrontime", parseDouble(fields[28]), parseDouble(fields[29]), parseDouble(fields[29]));
                String detectionMethod = fields[60] == "Radial Velocity" ? "RV" : "IDK";
                String[] time = fields[24].Split('-');
                String lastUpdate = String.Format("{0}/{1}/{2}", time[0].Substring(2, 2), time[1], time[2]);
                String discovery = fields[23];

                Planet newPlanet = new Planet(names, mass, period, semiMajorAxis, eccentricity, periastron, periastronTime, "", detectionMethod, lastUpdate, discovery);
                planets.Add(newPlanet);

            }
            return planets;
		}

        private Double parseDouble(String value)
        {
            return value == "" ? 0.0 : Double.Parse(value);
        }
	}
}

