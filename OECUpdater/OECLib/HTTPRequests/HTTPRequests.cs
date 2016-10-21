using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace OECLib.HTTPRequests
{
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

			//add the protocol to the start of our URL if it doesn't contain it
			if (!HttpsByDefault && !URL.StartsWith ("http://")) {
				URL = "http://" + URL;
			} else if (!URL.StartsWith ("https://")) {
				URL = "https://" + URL;
			}

			//create a new webclient for the request
			using (client = new WebClient ()) {
				//add headers if specified
				if(headers != null) {
					foreach (string header in headers) {
						client.Headers.Add (header);
					}
				}

				result = client.DownloadString (URL);

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

			if (!HttpsByDefault && !URL.StartsWith ("http://")) {
				URL = "http://" + URL;
			} else if (!URL.StartsWith ("https://")) {
				URL = "https://" + URL;
			}

			using (client = new WebClient ()) {
				//add headers if specified
				if (headers != null) {
					foreach (string header in headers) {
						client.Headers.Add (header);
					}
				}

				client.DownloadFile (URL, filename);
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
		public async Task DownloadFileAsync (string URL, string filename, 
			List<string> headers, bool HttpsByDefault=false) {

			if (!HttpsByDefault && !URL.StartsWith ("http://")) {
				URL = "http://" + URL;
			} else if (!URL.StartsWith ("https://")) {
				URL = "https://" + URL;
			}

			using (client = new WebClient ()) {
				//add headers if specified
				if (headers != null) {
					foreach (string header in headers) {
						client.Headers.Add (header);
					}
				}

				return client.DownloadFileTaskAsync (URL, filename);
			}
		}

	}
}

