using OECLib.Exoplanets;
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

		List<Planet> Run();
	}
}

