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
            return "NASA";
        }

        public string GetDescription()
        {
            return "This plugin allows the extraction of data from the NASA database";
        }

        public string GetAuthor()
        {
            return "Spazio";
        }

		public List<StellarObject> Run(String date, String sysName=null)
        {
            Dictionary<string, Star> stars = new Dictionary<string, Star>();
            HTTPRequest req = new HTTPRequest();
            string line;

            //string date = "2016-09-01";
            
            string columns = "pl_hostname,pl_name,pl_massj,pl_orbper,pl_orbsmax,pl_orbeccen,pl_orbeccenerr1,pl_orbeccenerr2,pl_orblper,pl_orblpererr1,pl_orblpererr2,pl_orbtper,pl_orbtpererr1,pl_orbtpererr2,pl_discmethod,rowupdate,pl_disc_refname";
            string filter = "rowupdate>to_date('" + date + "','yyyy-mm-dd')";
            if (sysName != null)
            {
                filter += String.Format("where=pl_hostname like {0}", sysName);
            }
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
                    star.AddStringMeasurement("name", starName);
                    stars.Add(starName, star);
                }
                Planet planet = new Planet();

                planet.AddStringMeasurement("name", fields[1]);

                //string[] altNames = fields[63].Split(',');

                String source = fields[1];

                planet.AddNumberMeasurement("mass", parseDouble(fields[2]));
                planet.AddNumberMeasurement("period", parseDouble(fields[3]));
                planet.AddNumberMeasurement("semimajoraxis", parseDouble(fields[4]));
                planet.AddNumberErrorMeasurement("eccentricity", parseDouble(fields[5]), parseDouble(fields[6]), parseDouble(fields[7]));
                planet.AddNumberErrorMeasurement("periastron", parseDouble(fields[8]), parseDouble(fields[9]), parseDouble(fields[10]));
                planet.AddNumberErrorMeasurement("periastrontime", parseDouble(fields[11]), parseDouble(fields[12]), parseDouble(fields[13]));
                planet.AddStringMeasurement("discoverymethod", fields[14]);
                planet.AddStringMeasurement("lastupdate", fields[15]);
                string[] year = fields[16].Split(' ');
                planet.AddStringMeasurement("discoveryyear", year[year.Length - 1]);

                star.AddChild(planet);

            }

			List<StellarObject> sys = new List<StellarObject>();
            sys.AddRange(stars.Values);
            return sys;
        }

        private double parseDouble(string value)
        {
            return value == "" ? 0.0 : double.Parse(value);
        }
    }
}
