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

		public bool DownloadFile (string URL, string filename) {
			try {
				client.DownloadFile (URL, filename);
				return true;
			} catch (WebException e) {
				return false;
			}
		}

	}
}

