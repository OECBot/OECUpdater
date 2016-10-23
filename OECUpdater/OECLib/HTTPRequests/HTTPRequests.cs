using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace OECLib.HTTPRequests
{
	/// <summary>
	/// HTTP request.
	/// </summary>
	public class HTTPRequest
	{
		WebClient client;

		/// <summary>
		/// Sends a request to a specified URL and returns the result.
		/// </summary>
		/// <returns>The response from the server as a string.</returns>
		/// <param name="URL">The URL to send a request to.</param>
		/// <param name="headers">Any HTTP headers the request should include. Okay to pass null.</param>
		/// <param name="HttpsByDefault">If true, send request through HTTPS. If false, send through HTTP. False by default.</param>
		public string RequestAsString (string URL, List<string> headers, bool HttpsByDefault=false)
		{
			string result = "";

			string newUrl = URL;

			//create a new webclient for the request
			using (client = new WebClient ()) {
				//add headers if specified
				if (headers != null) {
					AddHeadersToRequest (client, headers);
				}

				result = client.DownloadString (newUrl);

			}
			return result;
		}

		/// <summary>
		/// Downloads a file from a URL and saves it to the disk under the specified file name.
		/// Saved in current working directory.
		/// </summary>
		/// <param name="URL">The URL to send a request to.</param>
		/// <param name="filename">The name to save the file under.</param>
		/// <param name="headers">Any HTTP headers the request should include. Okay to pass null.</param>
		/// <param name="HttpsByDefault">If true, send request through HTTPS. If false, send through HTTP. False by default.</param>
		public void DownloadFile (string URL, string filename, 
			List<string> headers, bool HttpsByDefault=false) {

			string newUrl = AppendHTTPToURL (URL, HttpsByDefault);

			using (client = new WebClient ()) {
				//add headers if specified
				if (headers != null) {
					AddHeadersToRequest (client, headers);
				}

				client.DownloadFile (newUrl, filename);
			}
		}

		/// <summary>
		/// Downloads a file from a URL and saves it to the disk under the specified file name.
		/// Saved in current working directory. This method yields control to whoever called it while the file downloads.
		/// </summary>
		/// <returns>A promise to download the file. Use the await keyword to wait until the file is done downloading.</returns>
		/// <param name="URL">The URL to send a request to.</param>
		/// <param name="filename">The name to save the file under.</param>
		/// <param name="headers">Any HTTP headers the request should include. Okay to pass null.</param>
		/// <param name="HttpsByDefault">If true, send request through HTTPS. If false, send through HTTP. False by default.</param>
        ///
        /*
		public async Task DownloadFileAsync (string URL, string filename, 
			List<string> headers, bool HttpsByDefault=false) {

			string newUrl = AppendHTTPToURL (URL, HttpsByDefault);

			using (client = new WebClient ()) {
				//add headers if specified
				if (headers != null) {
					AddHeadersToRequest (client, headers);
				}
				return client.DownloadFileTaskAsync (newUrl, filename);
			}
		}
        */
		
		/// <summary>
		/// Adds headers to the request.
		/// </summary>
		/// <param name="client">The web client instance.</param>
		/// <param name="headers">The list of headers to add to the request.</param>
		void AddHeadersToRequest(WebClient client, List<string> headers){
			foreach (string header in headers) {
				client.Headers.Add (header);	
			}
		}

		/// <summary>
		/// Appends the HTTP text to URL if the given string doesn't already contain it.
		/// </summary>
		/// <returns>The new URL with the http text added or the original URL if it was already there.</returns>
		/// <param name="URL">The specified URL</param>
		/// <param name="HttpsByDefault">If true, append HTTPS. If false, append HTTP. False by default.</param>
		string AppendHTTPToURL(string URL, bool HttpsByDefault=false) {
			string newUrl;
			bool startsWithHeader = URL.StartsWith ("http://") || !URL.StartsWith ("https://");

			if (startsWithHeader) {
				if (!HttpsByDefault) {
					newUrl = "http://" + URL;
				} else {
					newUrl = "https://" + URL;
				}
				return newUrl;
			}

			return URL;
		}

	}
}

