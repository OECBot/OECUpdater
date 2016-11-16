using System;
using System.Collections.Generic;

using OECLib.Utilities;
using OECLib.Data;
using OECLib.Interface;
using System.IO;
using System.Text.RegularExpressions;

namespace NASAExoplanetPlugin
{
    public class NASAExoplanet : IPlugin
    {
        private string baseURL = "http://exoplanetarchive.ipac.caltech.edu/cgi-bin/nstedAPI/nph-nstedAPI?table=exoplanets&select={0}&where={1}";

        public void Initialize()
        {

        }

        public string GetName()
        {
            return "Exoplanet.EU";
        }

        public string GetDescription()
        {
            return "This plugin allows the extraction of data from the exoplanet.eu database";
        }

        public string GetAuthor()
        {
            return "Spazio";
        }

		public List<StellarObject> Run()
        {
            Dictionary<string, Star> stars = new Dictionary<string, Star>();
            HTTPRequest req = new HTTPRequest();
            string line;

            string date = "2016-09-01";
            string columns = "pl_hostname,pl_name,pl_massj,pl_orbper,pl_orbsmax,pl_orbeccen,pl_orbeccenerr1,pl_orbeccenerr2,pl_orblper,pl_orblpererr1,pl_orblpererr2,pl_orbtper,pl_orbtpererr1,pl_orbtpererr2,pl_discmethod,rowupdate,pl_disc_refname";
            string filter = "rowupdate>to_date('" + date + "','yyyy-mm-dd')";
            string newUrl = string.Format(baseURL, columns, filter);
            string httpResponse = req.RequestAsString(newUrl, null);

            StringReader sr = new StringReader(httpResponse);
            sr.ReadLine();

            while ((line = sr.ReadLine()) != null)
            {

                string[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                Star star = new Star();
                //todo: check if file exists and update that instead
                string starName = fields[0];
                if (stars.ContainsKey(starName))
                    star = stars[starName];
                else
                {
                    star.AddMeasurement("name", starName);
                    stars.Add(starName, star);
                }
                Planet planet = new Planet();

                planet.AddMeasurement("name", fields[1]);


				KeyValuePair<string, string> errMinus = new KeyValuePair<string, string> ("errorMinus", "");
				KeyValuePair<string, string> errPlus = new KeyValuePair<string, string> ("errorPlus", "");

                //string[] altNames = fields[63].Split(',');

                String source = fields[1];

                planet.AddMeasurement("mass", fields[2]);
                planet.AddMeasurement("period", fields[3]);
                planet.AddMeasurement("semimajoraxis", fields[4]);

				planet.AddMeasurement("eccentricity", fields[5], new Dictionary<string, string> {
					{ "errorplus", fields[6]},
					{ "errorminus", fields[7]},
				});
				planet.AddMeasurement("periastron", fields[8], new Dictionary<string, string> {
					{ "errorplus", fields[9]},
					{ "errorminus", fields[10]},
				});
				planet.AddMeasurement("periastrontime", fields[11],  new Dictionary<string, string> {
					{ "errorplus", fields[12]},
					{ "errorminus", fields[13]},
				});

                planet.AddMeasurement("discoverymethod", fields[14]);
                planet.AddMeasurement("lastupdate", fields[15]);
                string[] year = fields[16].Split(' ');
                planet.AddMeasurement("discoveryyear", year[year.Length - 1]);

                star.AddChild(planet);

            }

			List<StellarObject> sys = new List<StellarObject>();
            sys.AddRange(stars.Values);
            return sys;
        }
    }
}
