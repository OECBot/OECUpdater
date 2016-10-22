using System;
using IronPython.Hosting;
using OECLib.Exoplanets.Units;
using System.Diagnostics;

namespace OECUpdater
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var ipy = Python.CreateRuntime();
			dynamic test = ipy.UseFile("Plugins/ExoplanetEU.py");
			test.Run();
		}
	}
}
