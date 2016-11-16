using OECLib.Data;
using System;
using System.Collections.Generic;

namespace OECLib.Interface
{
	
	public interface IPlugin
	{
		String GetName();

		String GetDescription();

        String GetAuthor();

		void Initialize();

		List<StellarObject> Run(String date, String name);
	}
}

