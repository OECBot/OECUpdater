using System;
using NASAExoplanetPlugin;
using OECLib.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
	public class NASAExoplanetUnitTests
	{
		NASAExoplanet nasaexoplanet = new NASAExoplanet();

		public void GetNameTest()
		{
			string value = nasaexoplanet.GetName();
			Assert.AreEqual("Exoplanet.NASA", value);
		}

		public void GetDescriptionTest()
		{
			string value = nasaexoplanet.GetDescription();
			Assert.AreEqual("This plugin allows the extraction of data from the NASA Exoplanet Archive", value);
		}

		public void GetAuthorTest()
		{
			string value = nasaexoplanet.GetAuthor();
			Assert.AreEqual("Spazio", value);
		}

		public void RunTest()
		{
			//Dictionary<string, Star> stars = new Dictionary<string, Star>();
			//List<Star> starlist = new List<Star>();
			//starlist.AddRange(stars.Values);

			List<Star> starlist = nasaexoplanet.Run();
			List<Star> expectedlist = nasaexoplanet.Run();

			for (int i = 0; i < starlist.Count; i = i + 1)
			{
				Assert.AreEqual(expectedlist[i], starlist[i]);
			}
		}
	}
}
