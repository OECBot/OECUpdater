using System;

namespace OECLib.Plugins
{
	public interface IPlugin
	{
		String GetName();

		String GetDescription();

		void Initialize();

		void Run();
	}
}

