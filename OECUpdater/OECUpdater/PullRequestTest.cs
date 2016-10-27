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
        public static Session session;

        public static void Main(string[] args)
        {
                       
            GitHubClient g = new GitHubClient(new ProductHeaderValue("SpazioApp"));
            string csrf = Membership.GeneratePassword(24, 1);
            //Session["CSRF:State"] = csrf;

            var request = new OauthLoginRequest(Session.clientId)
            {
                Scopes = { "user", "notifications", "repo" },
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
            PullRequest(s);

        }

        public async static void PullRequest(Session s) {
            bool hasAccess = await CheckAccessAsync(s);
            Console.WriteLine(hasAccess ? "You are a contributer of this repo!" : "You don't have access to this repo and cannot use this application!");
            if (!hasAccess)
            {
                return;
            }
            var repo = await s.client.Repository.Get("Gazing", "RedditPostsSaver");
            try
            {
                NewPullRequest newPullRequest = new NewPullRequest("Test123", "test", "master");
                PullRequest pr = await s.client.PullRequest.Create(repo.Id, newPullRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async static Task<bool> CheckAccessAsync(Session s)
        {
            ApiOptions options = new ApiOptions();
            var repos = await s.client.Repository.GetAllContributors("Gazing", "RedditPostsSaver");
            User cur = await s.client.User.Current();
            int id = cur.Id;
            foreach (RepositoryContributor person in repos)
            {
                if (id == person.Id)
                {
                    return true;
                }
                //Console.WriteLine("{0} : {1}", person.Id, id);
            }
            return false;
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
