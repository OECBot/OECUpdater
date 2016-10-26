using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace OECUpdater
{
    class PullRequestTest
    {
        public static TcpListener listener;

        public static void Main(string[] args)
        {
            Dictionary<string, string> Session = new Dictionary<string, string>();
            string clientId = "8c533ca43cbfba2e2268";
            string clientSecret = "fdfa25888fb0f815c248114fba5f7f26d834743f";
            string localHost = "localhost";
            int port = 4567;
            
            GitHubClient g = new GitHubClient(new ProductHeaderValue("SpazioApp"));
            string csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            var request = new OauthLoginRequest(clientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf,
            };

            String oauthLoginUrl = g.Oauth.GetGitHubLoginUrl(request).ToString();
            System.Diagnostics.Process.Start(oauthLoginUrl);
            
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();
            
            listener.BeginAcceptTcpClient(onAcceptConnection, null);

            Console.ReadKey();
        }

        private static void onAcceptConnection(IAsyncResult asyn)
        {
            TcpClient client = listener.EndAcceptTcpClient(asyn);
            Console.WriteLine("Client connected.");

            byte[] rq = new byte[8192];
            NetworkStream stream = client.GetStream();

            stream.Read(rq, 0, 8192);

            Console.WriteLine(Encoding.UTF8.GetString(rq));

            byte[] resp;

            if (!Encoding.UTF8.GetString(rq).Split(' ')[1].Contains("callback"))
            {
                resp = Encoding.UTF8.GetBytes("Hello There");
                client.GetStream().Write(resp, 0, resp.Length);
                client.GetStream().Flush();
                client.Close();
                Console.WriteLine("Non-Callback detected. Nulling request.");
                listener.BeginAcceptTcpClient(onAcceptConnection, null);
                return;
            }

            String code = Encoding.UTF8.GetString(rq).Split(' ')[1].Split('=')[1].Split('&')[0];

            resp = Encoding.UTF8.GetBytes("Authorization successful. You can now close this web page.\n"+code);

            client.GetStream().Write(resp, 0, resp.Length);
            client.GetStream().Flush();

            Console.WriteLine("response sent");

            client.Close();
            //listener.BeginAcceptTcpClient(onAcceptConnection, null);
            listener.Stop();
        }
    }
}
