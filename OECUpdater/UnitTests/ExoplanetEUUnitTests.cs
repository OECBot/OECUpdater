using System;
using ExoplanetEU;
using OECLib.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
	[TestFixture]
	public class ExoplanetEUUnitTests
	{
		ExoplanetEUPlugin exoplaneteu = new ExoplanetEUPlugin();

		[Test]
		public void GetNameTest()
		{
			string value = exoplaneteu.GetName();
			Assert.AreEqual("Exoplanet.EU", value);
		}

		[Test]
		public void GetDescriptionTest()
		{
			string value = exoplaneteu.GetDescription();
			Assert.AreEqual("This plugin allows the extraction of data from the exoplanet.eu database", value);
		}

		[Test]
		public void GetAuthorTest()
		{
			string value = exoplaneteu.GetAuthor();
			Assert.AreEqual("Spazio", value);
		}

		[Test]
		public void RunTest()
		{
			List<Planet> starlist = exoplaneteu.Run();
			List<Planet> expectedlist = exoplaneteu.Run();

			for (int i = 0; i < starlist.Count; i = i + 1)
			{
				Assert.AreEqual(expectedlist[i], starlist[i]);
			}
		}
	}
}
