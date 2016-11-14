using System;
using NASAExoplanetPlugin;
using OECLib.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
	[TestFixture]
	public class NASAExoplanetUnitTests
	{
		NASAExoplanet nasaexoplanet = new NASAExoplanet();

		[Test]
		public void GetNameTest()
		{
			string value = nasaexoplanet.GetName();
			Assert.AreEqual("Exoplanet.NASA", value);
		}

		[Test]
		public void GetDescriptionTest()
		{
			string value = nasaexoplanet.GetDescription();
			Assert.AreEqual("This plugin allows the extraction of data from the NASA Exoplanet Archive", value);
		}

		[Test]
		public void GetAuthorTest()
		{
			string value = nasaexoplanet.GetAuthor();
			Assert.AreEqual("Spazio", value);
		}

		[Test]
		public void RunTest()
		{
			List<Star> starlist = nasaexoplanet.Run();
			List<Star> expectedlist = nasaexoplanet.Run();

			for (int i = 0; i < starlist.Count; i = i + 1)
			{
				Assert.AreEqual(expectedlist[i], starlist[i]);
			}
		}
	}
}
