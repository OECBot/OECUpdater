using System;
using System.Net;

namespace OECLib.WebRequests
{
	public class WebRequest
	{
		public string MakeRequest (string URL)
		{
			WebRequest request = WebRequest.Create (URL);
			return "lol";
		}




	}
}

