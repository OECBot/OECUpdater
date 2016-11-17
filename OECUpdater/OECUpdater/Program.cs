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


//            XMLDemo();
//            PlanetSystem loadedSystem = Serializer.LoadXMLFile("test.xml");
//            runLoop(new TimeSpan(23, 59, 59), TimeSpan.FromHours(24));

			Console.WriteLine ("In Program.cs");

//			XmlDocument doc = new XmlDocument ();
//			doc.LoadXml (File.ReadAllText ("test.xml"));
//			Console.WriteLine (doc.ToString ());

			PlanetMerger.TestMerge();
        }

        /// <summary>
		/// Runs all plugins at the specified time interval.
		/// </summary>
        /// <param name="startTime">The time when the timer will begin</param>
		/// <param name="interval">The periodic length of time, after the initial startTime, that the plugins will run.</param>
//        private static void runLoop(TimeSpan startTime, TimeSpan interval)
//        {
//            TimeSpan current = DateTime.Now.TimeOfDay;
//            TimeSpan difference = startTime - current;
//
//            var timer = new System.Threading.Timer((e) => { runPlugins();}, null,
//                Convert.ToInt32(difference.TotalMilliseconds), 
//                Convert.ToInt32(interval.TotalMilliseconds));
//
//            while (true)
//            {
//
//            }
//            
//        }

//        /// <summary>
//		/// Run all of the plugins.
//		/// </summary>
//        private static void runPlugins()
//        {
//            Serializer.InitPlugins();
//
//            // Run each plugin
//            foreach (var pluginName in pluginNames)
//            {
//                // Retrieve the list of planets that have been most recently updated
//                List<Planet> planets = Serializer.plugins[pluginName].Run();
//                Console.WriteLine("Running " + pluginName + " plugin");
//                Console.ReadKey();
//
//                // Demo showing each planet that has been updated since 09/01/2016
//                //foreach (var planet in planets)
//                //{
//                //    StringBuilder output = new StringBuilder();
//                //    XmlWriterSettings ws = new XmlWriterSettings();
//                //    ws.Indent = true;
//                //    ws.OmitXmlDeclaration = true;
//
//                //    using (XmlWriter xw = XmlWriter.Create(output, ws))
//                //    {
//                //        planet.Write(xw);
//                //        xw.Flush();
//                //    }
//                //    Console.WriteLine(output.ToString());
//                //    Console.ReadKey();
//                //}
//            }
//        }
	}
}
