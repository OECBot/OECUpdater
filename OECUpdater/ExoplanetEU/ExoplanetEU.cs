using System;
using System.Xml;

using OECLib.Plugins;

namespace ExoplanetEU
{
	public class ExoplanetEU : IPlugin
	{
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

		public void Run()
		{
			Console.WriteLine("{0}: {1}", GetName(), GetDescription());
		}
	}
}

