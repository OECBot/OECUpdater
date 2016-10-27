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
        public static string localHost = "127.0.0.1";
        public static int port = 4567;
        public static string menu = "1)Check Access\n2)Display all pull-requests for repository\n3)Create Pull-Request for a repository\n4)Set Current Repository\n5)Logout";
        public static Dictionary<String, Func<Task>> commands = new Dictionary<string, Func<Task>>();
        public static Session s;

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello welcome to Spazio Demo!");
            Console.WriteLine("Would you like to continue and login? Y/N");
            initalizeCommands();
            String resp = Console.ReadLine();
            while (resp != "Y" && resp != "N")
            {
                Console.WriteLine("Enter either Y or N.");
                resp = Console.ReadLine();
            }
            if (resp == "N")
            {
                return;
            }

            GitHubClient g = new GitHubClient(new ProductHeaderValue("SpazioApp"));
            string csrf = Membership.GeneratePassword(24, 1);

            var request = new OauthLoginRequest(Session.clientId)
            {
                Scopes = { "user", "notifications", "repo" },
                State = csrf,
            };

            String oauthLoginUrl = g.Oauth.GetGitHubLoginUrl(request).ToString();
            System.Diagnostics.Process.Start(oauthLoginUrl);
            

            BeginGitHubSession(g);


            while (1 == 1)
            {

            }
            
        }

        public static void initalizeCommands()
        {
            commands.Add("1", CheckAccess);
        }


        public async static void BeginGitHubSession(GitHubClient client)
        {
            CallBackServer server = new CallBackServer(localHost, port, client);
            s = await server.Start();
            Console.WriteLine(s.ToString());
            Console.WriteLine("GitHub session started...");

            while (1 == 1)
            {
                Console.WriteLine("Type a command!");
                Console.WriteLine(menu);
                Console.WriteLine("Command: ");
                String cmd = Console.ReadLine();
                Console.WriteLine(cmd);
                while (!commands.ContainsKey(cmd))
                {
                    Console.WriteLine("Type a correct command...");
                    Console.WriteLine(menu);
                    Console.WriteLine("Command: ");
                    cmd = Console.ReadLine();
                }
                await commands[cmd]();
            }

        }

        public async static Task CheckAccess()
        {
            Console.WriteLine("Repository Owner: ");
            String owner = Console.ReadLine();
            Console.WriteLine("Repository Name: ");
            String name = Console.ReadLine();

            bool hasAccess = await CheckAccessAsync(owner, name);
            Console.WriteLine(hasAccess ? "You are a contributer of this repo!" : "You don't have access to this repo and cannot use this application!");
        }

        public async static void PullRequest() {
            bool hasAccess = await CheckAccessAsync("Gazing", "RedditPostsSaver");
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

        public async static void AllPullRequests()
        {
            var repo = await s.client.Repository.Get("Gazing", "RedditPostsSaver");
            var prs = await s.client.PullRequest.GetAllForRepository(repo.Id, new ApiOptions());
            foreach (PullRequest pr in prs)
            {
                Console.WriteLine(pr.Title);
            }
        }

        public async static Task<bool> CheckAccessAsync(String owner, String name)
        {
            ApiOptions options = new ApiOptions();
            var repos = await s.client.Repository.GetAllContributors(owner, name);
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
