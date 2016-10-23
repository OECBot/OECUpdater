﻿using System;
using System.Xml;

using OECLib.Plugins;
using OECLib.Exoplanets;
using System.Collections.Generic;

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

		public String GetAuthor()
		{
			return "Spazio";
		}

		public List<Planet> Run()
		{
            return null;
		}
	}
}

