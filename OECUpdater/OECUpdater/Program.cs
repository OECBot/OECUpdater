using System;
using IronPython.Hosting;

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
