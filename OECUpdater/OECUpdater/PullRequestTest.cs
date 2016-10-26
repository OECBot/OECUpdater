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
using OECLib.Utilities;
using OECLib.GitHub;

namespace OECUpdater
{
    class PullRequestTest
    {
        public static TcpListener listener;
        public static string localHost = "127.0.0.1";
        public static int port = 4567;

        public static void Main(string[] args)
        {
                       
            GitHubClient g = new GitHubClient(new ProductHeaderValue("SpazioApp"));
            string csrf = Membership.GeneratePassword(24, 1);
            //Session["CSRF:State"] = csrf;

            var request = new OauthLoginRequest(Session.clientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf,
            };

            String oauthLoginUrl = g.Oauth.GetGitHubLoginUrl(request).ToString();
            System.Diagnostics.Process.Start(oauthLoginUrl);
            

            Authorize(g);
            
            Console.ReadKey();
        }


        public async static void Authorize(GitHubClient client)
        {
            CallBackServer server = new CallBackServer(localHost, port, client);
            Session s = await server.Start();
            Console.WriteLine(s.ToString());
        }

        /*
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
         */
    }
}
