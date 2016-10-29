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
using OECLib.Plugins;
using OECLib;

namespace OECUpdater
{
    public class DemoSprint2
    {
        public static string localHost = "127.0.0.1";
        public static int port = 4567;
        public static string menu = "1)Check Access\n2)Display all pull-requests for repository\n3)Create Pull-Request for a repository\n4)Set Current Repository\n5)Logout";
        public static Dictionary<String, Func<Task>> commands = new Dictionary<string, Func<Task>>();
        public static Session s;
        public static OECBot bot;

        public static Dictionary<String, IPlugin> plugins = new Dictionary<string, IPlugin>();

        public static void Main(string[] args)
        {
            initalizeCommands();
            Serializer.InitPlugins();
            plugins = Serializer.plugins;

            Console.WriteLine("Hello welcome to Spazio Demo!");
            Console.WriteLine("Would you like to continue and login? Y/N");
            

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

            Task console = BeginGitHubSession(g);
            console.Wait();

        }

        public static void initalizeCommands()
        {
            commands.Add("1", CheckAccess);
            commands.Add("2", runBot);
            commands.Add("3", getFile);
            commands.Add("4", createRepo);
        }

        public async static Task createRepo()
        {
            Console.WriteLine("Enter a name for the new branch!");
            String name = Console.ReadLine();
            RepositoryManager rm = new RepositoryManager(s, await s.client.Repository.Get("Gazing", "OECTest"));
            try
            {
                String branch = await rm.createBranch(name);
                Console.WriteLine("Successfully created branch named: {0}", branch);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async static Task runBot()
        {
            Console.WriteLine("Starting Bot");
            var repo = await s.client.Repository.Get("Gazing", "OECTest");
            bot = new OECBot(getPlugins(), repo);
            bot.Start();
        }

        public async static Task getFile()
        {
            RepositoryManager rm = new RepositoryManager(s, await s.client.Repository.Get("Gazing", "OECTest"));
            try
            {
                String content = await rm.getFile("24 Sex.xml");
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async static Task BeginGitHubSession(GitHubClient client)
        {
            CallBackServer server = new CallBackServer(localHost, port, client);
            try
            {
                s = await server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(s.ToString());
            Console.WriteLine("GitHub session started...");

            while (1 == 1)
            {
                Console.WriteLine("Type a command!");
                Console.WriteLine(menu);
                Console.WriteLine("Command: ");
                String cmd = Console.ReadLine();
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

        public static List<IPlugin> getPlugins()
        {
            List<IPlugin> plugs = new List<IPlugin>();
            foreach (String name in plugins.Keys)
            {
                plugs.Add(plugins[name]);
            }
            return plugs;
        }

        public async static Task CheckAccess()
        {
            Console.WriteLine("Repository Owner: ");
            String owner = Console.ReadLine();
            Console.WriteLine("Repository Name: ");
            String name = Console.ReadLine();

            bool hasAccess = await CheckAccessByName(owner, name);
            Console.WriteLine(hasAccess ? "You are a contributer of this repo!" : "You don't have access to this repo and cannot use this application!");
        }

        public async static void PullRequest()
        {
            bool hasAccess = await CheckAccessByName("Gazing", "RedditPostsSaver");
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

        public async static Task<bool> CheckAccessByName(String owner, String name)
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
    }
}
