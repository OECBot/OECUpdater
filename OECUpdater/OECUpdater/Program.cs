using System;
using IronPython.Hosting;
using OECLib.Exoplanets.Units;
using System.Diagnostics;
using System.Xml;
using System.Text;

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
            XmlWriter xw = XmlWriter.Create(output, ws);
            UnitError test = new UnitError("test", 12.3, 0.4, 0.4);
            test.Write(xw);
            xw.Flush();
            Console.WriteLine(output.ToString());
            Console.ReadKey();
		}
	}
}
