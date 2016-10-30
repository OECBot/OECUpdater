using OECLib.Exoplanets;
﻿using OECLib.Data;
using System;
using System.Collections.Generic;

namespace OECLib.Plugins
{
	public interface IPlugin
	{
		String GetName();

		String GetDescription();

        	String GetAuthor();

		void Initialize();

		List<Exoplanets.Planet> Run();
	}
	
	public interface INewPlugin
	{
		String GetName();

		String GetDescription();

        	String GetAuthor();

		void Initialize();

		List<Data.Star> Run();
	}
}

