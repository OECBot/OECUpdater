using System;
using System.Collections.Generic;
using OECLib.Utilities;
using NUnit.Framework;

namespace UnitTests
{
	[TestFixture]
	public class HTTPRequestsUnitTests
	{
		private HTTPRequest httprequest;
		private static string URLEU;
		private static string URLNASA;
		private static List<string> headerslist;
		private static string notheaderslist;
		private static List<double> nottypeheaderslist;

		[SetUp]
		protected void SetUp()
		{
			httprequest = new HTTPRequest();
			URLEU = "http://exoplanet.eu/catalog/csv/?status=&f={0}";
			URLNASA = "http://exoplanetarchive.ipac.caltech.edu/cgi-bin/nstedAPI/nph-nstedAPI?table=exoplanets&select={0}&where={1}";
			headerslist.Add("Header");
			headerslist.Add("123");
			notheaderslist = "This is a string";
			nottypeheaderslist.Add(1.0);
			nottypeheaderslist.Add(10);
		}

		[TestCase("", null, false)]
		[TestCase("", null, true)]
		public void RequestAsStringEmptyURLTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("", value);
		}

		[TestCase("ww,abc/123", null, false)]
		[TestCase("ww,abc/123", null, true)]
		public void RequestAsStringInvalidURLTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("", value);
		}

		[TestCase(2, null, false)]
		[TestCase(10.0, null, true)]
		public void RequestAsStringInvalidTypeURLTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("", value);
		}

		[TestCase(URLNASA, null, false)]
		[TestCase(URLNASA, null, true)]
		[TestCase(URLEU, null, false)]
		[TestCase(URLEU, null, true)]
		public void RequestAsStringNoHeadersTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("", value);
		}

		[TestCase(URLNASA, notheaderslist, false)]
		[TestCase(URLNASA, notheaderslist, true)]
		[TestCase(URLEU, notheaderslist, false)]
		[TestCase(URLEU, notheaderslist, true)]
		public void RequestAsStringInvalidHeadersTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("", value);
		}

		[TestCase(URLNASA, nottypeheaderslist, false)]
		[TestCase(URLNASA, nottypeheaderslist, true)]
		[TestCase(URLEU, nottypeheaderslist, false)]
		[TestCase(URLEU, nottypeheaderslist, true)]
		public void RequestAsStringInvalidTypeHeadersTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("", value);
		}

		[TestCase(URLNASA, headerslist, false)]
		[TestCase(URLNASA, headerslist, true)]
		[TestCase(URLEU, headerslist, false)]
		[TestCase(URLEU, headerslist, true)]
		public void RequestAsStringWithHeadersTest(string URL, List<string> headers, bool HttpsByDefault = false)
		{
			string value = httprequest.RequestAsString(URL, headers, HttpsByDefault);
			Assert.AreEqual("Header, 123", value);
		}
	}
}
