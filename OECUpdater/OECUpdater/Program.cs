using System;
using IronPython.Hosting;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using OECLib.Utilities;
using OECLib.Interface;
using System.IO;


namespace OECUpdater
{
	class MainClass
	{
        private static readonly String[] pluginNames = new String[1] {"Exoplanet.EU"};

        public static void Main (string[] args)
		{
            /*
			var ipy = Python.CreateRuntime();
			dynamic test = ipy.UseFile("Plugins/ExoplanetEU.py");
			test.Run();
             * */
			Console.WriteLine ("In Program.cs");

			NASAExoplanetPlugin.NASAExoplanet plugin = new NASAExoplanetPlugin.NASAExoplanet();
			ExoplanetEU.ExoplanetEU plugin2 = new ExoplanetEU.ExoplanetEU ();

			List<OECLib.Data.StellarObject> stars = plugin.Run ("2016-11-01");
			List<OECLib.Data.StellarObject> starEU = plugin2.Run ("2016-11-01");
			foreach (OECLib.Data.StellarObject star in stars) {
				Console.WriteLine(star.XMLTag(new XmlDocument()).OuterXml);
				Console.Write ('\n');
			}

			foreach (OECLib.Data.StellarObject star in starEU) {
				Console.WriteLine(star.XMLTag(new XmlDocument()).OuterXml);
				Console.Write ('\n');
			}
			//PlanetMerger.TestMerge();
        }

        
	}
}
