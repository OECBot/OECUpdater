using System;
using System.Net;

namespace OECLib.HTTPRequests
{
	public class HTTPRequest
	{
		WebClient client;

		public HTTPRequest () {
			client = new WebClient ();
		}

		public string RequestAsString (string URL)
		{
			return client.DownloadString (URL);
		}

		public void DownloadFile (string URL, string filename) {
			client.DownloadFile (URL, filename);
		}

	}
}

