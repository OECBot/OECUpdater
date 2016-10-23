using System;
using IronPython.Hosting;
using OECLib.Exoplanets.Units;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using OECLib.Exoplanets;
using OECLib.Utilities;
using OECLib.Plugins;

namespace OECUpdater
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            /*
			var ipy = Python.CreateRuntime();
			dynamic test = ipy.UseFile("Plugins/ExoplanetEU.py");
			test.Run();
             * */
            
            StringBuilder output = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.OmitXmlDeclaration = true;


            // Exmaple using info from  11 Com.xml in OEC
            UnitError distance1 = new UnitError("distance", 88.9, 1.7, 1.7);

            // Star info
            List<String> names = new List<string>();
            names.Add("11 Com");
            names.Add("11 Comae Berenices");
            names.Add("HD 107383");
            names.Add("HIP 60202");
            names.Add("TYC 1445-2560-1");
            names.Add("SAO 100053");
            names.Add("HR 4697");
            names.Add("BD+18 2592");
            names.Add("2MASS J12204305+1747341");
            UnitError mass1 = new UnitError("mass", 2.7, 0.3, 0.3);
            UnitError radius1 = new UnitError("radius", 19, 2.0, 2.0);
            UnitError magB1 = new UnitError("magB", 5.74, 0.02, 0.02);
            UnitError magJ1 = new UnitError("magJ", 2.943, 0.334, 0.334);
            UnitError magH1 = new UnitError("magH", 2.484, 0.268, 0.268);
            UnitError magK1 = new UnitError("magK", 2.282, 0.346, 0.346);
            UnitError temeprature1 = new UnitError("temperature", 4742.0, 100.0, 100.0);
            UnitError metallicity1 = new UnitError("metallicity", -0.35, 0.09, 0.09);

            // Planet info
            List<String> p1Names = new List<string>();
            p1Names.Add("11 Com b");
            UnitError p1Mass = new UnitError("mass", 19.4, 1.5, 1.5, "msini");
            UnitError p1Period = new UnitError("perios", 326.03, 0.32, 0.32);
            UnitError p1Semimajor = new UnitError("semimajoraxis", 1.29, 0.05, 0.05);
            UnitError p1Ecc = new UnitError("eccentricity", 0.231, 0.05, 0.05);
            UnitError p1Per = new UnitError("periastron", 94.8, 1.5, 1.5);
            UnitError p1PerTime = new UnitError("periastrontime", 2452899.6, 1.6, 1.6);

            List<Planet> s1Planets = new List<Planet>();
            Planet planet1 = new Planet(p1Names, p1Mass, p1Period, p1Semimajor, p1Ecc, p1Per, p1PerTime,
                "11 Com b is a brown dwarf-mass companion to the intermediate-mass star 11 Comae Berenices.",
                "RV", "15/09/20", "2008");
            s1Planets.Add(planet1);


            Star star1 = new Star(names, mass1, radius1, 4.74f, magB1, magJ1, magH1, magK1, temeprature1, metallicity1, "G8 III", s1Planets);
            PlanetSystem system1 = new PlanetSystem("11 Com", "12 20 43", "+17 47 34", distance1, star1);

            using (XmlWriter xw = XmlWriter.Create(output, ws))
            {
                system1.Write(xw);
                xw.Flush();
            }
            Console.WriteLine(output.ToString());
            Console.ReadKey();
        }
	}
}
