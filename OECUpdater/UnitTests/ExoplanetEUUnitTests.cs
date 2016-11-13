using System;
using ExoplanetEU;
using OECLib.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
	public class ExoplanetEUUnitTests
	{
		ExoplanetEUPlugin exoplaneteu = new ExoplanetEUPlugin();

		public void GetNameTest()
		{
			string value = exoplaneteu.GetName();
			Assert.AreEqual("Exoplanet.EU", value);
		}

		public void GetDescriptionTest()
		{
			string value = exoplaneteu.GetDescription();
			Assert.AreEqual("This plugin allows the extraction of data from the exoplanet.eu database", value);
		}

		public void GetAuthorTest()
		{
			string value = exoplaneteu.GetAuthor();
			Assert.AreEqual("Spazio", value);
		}

		public void RunTest()
		{
			//Dictionary<string, Star> stars = new Dictionary<string, Star>();
			//List<Star> starlist = new List<Star>();
			//starlist.AddRange(stars.Values);

			List<Star> starlist = exoplaneteu.Run();
			List<Star> expectedlist = exoplaneteu.Run();

			for (int i = 0; i < starlist.Count; i = i + 1)
			{
				Assert.AreEqual(expectedlist[i], starlist[i]);
			}
		}
	}
}
