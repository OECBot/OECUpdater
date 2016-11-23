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

		//info for planets
		//semimajoraxis
		//separation
		//eccentricity
		//perastrion
		//longitude
		//meananomaly
		//ascendingnode
		//inclination
		//impact parameter
		//period
		//transittime
		//periatriontime
		//maximumrvtime
		//mass (jupiter mass)
		//radius (jupiter radius)
		//temperature
		//age
		//spectraltype
		//magb,v,r,i,j,h,k
		//discovery method
		//is transiting
		//description
		//discoveryyear
		//lastupdate
		//spinorbitalignment
		string planetColumns = "pl_hostname,pl_name," +
			"pl_orbsmax,pl_orbsmaxerr1,pl_orbsmaxerr2," + //semimajor axis +/-
			"pl_orbeccen,pl_orbeccenerr1,pl_orbeccenerr2," + //eccentricity +/-
			"pl_orblper,pl_orblpererr1,pl_orblpererr2," + //periastron +/-
			"st_elon," + //longitude
			"pl_orbincl,pl_orbinclerr1,pl_orbinclerr2," + //inclination
			"pl_imppar,pl_impparerr1,pl_impparerr2," + //impact parameter
			"pl_orbper,pl_orbpererr1,pl_orbpererr2," + //period days
			"pl_tranflag,pl_trandur,pl_trandurerr1,pl_trandurerr2," + //transit flag and duration
			"pl_orbtper,pl_orbtpererr1,pl_orbtpererr2," + //periastrontime +/-
			"pl_massj,pl_massjerr1,pl_massjerr2," + //mass (jupiter masses)
			"pl_radj,pl_radjerr1,pl_radjerr2," + //radius (juptier radii)
			"pl_eqt,pl_eqterr1,pl_eqterr2," + //temperature
			"pl_disc,pl_discmethod,rowupdate,pl_disc_refname," +
			"pl_msinij,pl_msinijerr1,pl_msinijerr2";

		//system info
		string sysColumns;


		//info for stars
		//mass (solar mass)
		//radius (solar radius)
		//temperature
		//age
		//metallicity
		//spectraltype
		//magb,v,r,i,j,h,k
		string starColumns = "pl_hostname," +
			"st_mass,st_masserr1,st_masserr2," + //mass (jupiter masses)
			"st_rad,st_raderr1,st_raderr2," + //radius (juptier radii)
			"st_teff,st_tefferr1,st_tefferr2," + //temperature
			"st_age,st_ageerr1,st_ageerr2," +
			"st_metfe,st_metfeerr1,st_metfeerr2," +
			"st_spstr," +
			"st_bj,st_vj,st_rc,st_ic,st_j,st_h,st_k";

        public void Initialize()
        {

        }

        public string GetName()
        {
            return "NASA Exoplanet Database";
        }

        public string GetDescription()
        {
            return "This plugin allows the extraction of data from the NASA Exoplanet Database";
        }

        public string GetAuthor()
        {
            return "Spazio";
        }

		public List<StellarObject> Run(String date, String sysName=null)
        {
            HTTPRequest req = new HTTPRequest();
            string line;

			Dictionary<string, Star> stars = updateStars(req, date);
            
            string filter = "rowupdate>to_date('" + date + "','yyyy-mm-dd')";
            if (sysName != null)
            {
                filter += String.Format("and pl_hostname=\'{0}\'", sysName);
            }
			string newUrl = string.Format(baseURL, planetColumns, filter);
			string httpResponse = req.RequestAsString (newUrl, null);
            StringReader sr = new StringReader(httpResponse);
            sr.ReadLine();

            while ((line = sr.ReadLine()) != null)
            {
				Star star;
                string[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                string starName = fields[0];

				if (stars.ContainsKey (starName))
					star = stars [starName];
                else
					continue;
				
                Planet planet = new Planet();

                planet.AddMeasurement("name", fields[1]);
				planet.Source = string.Format("http://exoplanetarchive.ipac.caltech.edu/cgi-bin/DisplayOverview/nph-DisplayOverview?objname={0}", fields[1].Replace(' ', '+'));

				string detectionMethod = fields [38].ToLower();
				bool transit = false;
				bool rv = false;

				if (detectionMethod == "transit") {
					transit = true;
				} else if (detectionMethod == "radial velocity") {
					detectionMethod = "RV";
					rv = true;
				}

				planet.AddMeasurement("semimajoraxis", fields[2], new Dictionary<string, string> {
					{ "errorplus", fields[3]},
					{ "errorminus", fields[4]}
				});
				planet.AddMeasurement("eccentricity", fields[5], new Dictionary<string, string> {
					{ "errorplus", fields[6]},
					{ "errorminus", fields[7]}
				});
				planet.AddMeasurement("periastron", fields[8], new Dictionary<string, string> {
					{ "errorplus", fields[9]},
					{ "errorminus", fields[10]}
				});
				planet.AddMeasurement("longitiude", fields[11]);
				planet.AddMeasurement("inclination", fields[12], new Dictionary<string, string> {
					{ "errorplus", fields[13]},
					{ "errorminus", fields[14]}
				});
				planet.AddMeasurement("impactparamater", fields[15], new Dictionary<string, string> {
					{ "errorplus", fields[16]},
					{ "errorminus", fields[17]}
				});
				planet.AddMeasurement("period", fields[18], new Dictionary<string, string> {
					{ "errorplus", fields[19]},
					{ "errorminus", fields[20]}
				});
				planet.AddMeasurement("periastrontime", fields[25], new Dictionary<string, string> {
					{ "errorplus", fields[26]},
					{ "errorminus", fields[27]}
				});

				if (rv) {
					planet.AddMeasurement ("mass", fields [41], new Dictionary<string, string> {
						{ "errorplus", fields [42] },
						{ "errorminus", fields [43] },
						{ "type", "msini" }
					});
				} else {
					planet.AddMeasurement ("mass", fields [28], new Dictionary<string, string> {
						{ "errorplus", fields [29] },
						{ "errorminus", fields [30] }
					});
				}


				planet.AddMeasurement("radius", fields[31], new Dictionary<string, string> {
					{ "errorplus", fields[32]},
					{ "errorminus", fields[33]},
				});
				planet.AddMeasurement("temperature", fields[34], new Dictionary<string, string> {
					{ "errorplus", fields[35]},
					{ "errorminus", fields[36]}
				});

				if (transit) {
					planet.AddMeasurement ("istransitting", fields [21]);
					planet.AddMeasurement ("transittime", fields [22], new Dictionary<string, string> {
						{ "errorplus", fields [23] },
						{ "errorminus", fields [24] }
					});
				}

				string[] year = fields[37].Split(' ');
				planet.AddMeasurement("discoveryyear", year[year.Length - 1]);
                planet.AddMeasurement("lastupdate", fields[39]);
				planet.AddMeasurement ("detectionmethod", detectionMethod);
                star.AddChild(planet);

            }

			List<StellarObject> sys = new List<StellarObject>();
            sys.AddRange(stars.Values);
            return sys;
        }

		Dictionary<string, Star> updateStars(HTTPRequest req, string date) {
			string line;
			Dictionary<string, Star> starList = new Dictionary<string, Star> ();

			string filter = "rowupdate>to_date('" + date + "','yyyy-mm-dd')";
			string newUrl = string.Format(baseURL, starColumns, filter);
			string httpResponse = req.RequestAsString(newUrl, null);

			StringReader sr = new StringReader(httpResponse);
			sr.ReadLine();
			while ((line = sr.ReadLine ()) != null) {
				string[] fields = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				string name = fields [0];

				if(starList.ContainsKey(name))
					continue;
				
				Star star = new Star ();
				star.AddMeasurement ("name", name);
				star.AddMeasurement("mass", fields[1], new Dictionary<string, string> {
					{ "errorplus", fields[2]},
					{ "errorminus", fields[3]},
				});
				star.AddMeasurement("radius", fields[4], new Dictionary<string, string> {
					{ "errorplus", fields[5]},
					{ "errorminus", fields[6]},
				});
				star.AddMeasurement("temperature", fields[7], new Dictionary<string, string> {
					{ "errorplus", fields[8]},
					{ "errorminus", fields[9]},
				});
				star.AddMeasurement ("age", fields[10], new Dictionary<string, string> {
					{ "errorplus", fields[11]},
					{ "errorminus", fields[12]},
				});
				star.AddMeasurement ("metallicity", fields[13], new Dictionary<string, string> {
					{ "errorplus", fields[14]},
					{ "errorminus", fields[15]},
				});
				star.AddMeasurement ("spectraltype", fields[16]);

				star.AddMeasurement ("magB", fields[17]);
				star.AddMeasurement ("magV", fields[18]);
				star.AddMeasurement ("magR", fields[19]);
				star.AddMeasurement ("magI", fields[20]);
				star.AddMeasurement ("magJ", fields[21]);
				star.AddMeasurement ("magH", fields[22]);
				star.AddMeasurement ("magK", fields[23]);
				
				starList.Add (name, star);
			}
			return starList;
		}
    }
}
