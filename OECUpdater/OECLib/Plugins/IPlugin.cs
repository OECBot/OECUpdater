using OECLib.Exoplanets;
using System;

namespace OECLib.Plugins
{
	public interface IPlugin
	{
		String GetName();

		String GetDescription();

        String GetAuthor();

		void Initialize();

		Planet Run();
	}
}

