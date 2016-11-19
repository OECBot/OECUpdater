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
using OECLib.Interface;
using OECLib;

namespace OECUpdater
{
    public class Demo
    {
        public static string localHost = "127.0.0.1";
        public static int port = 4567;
        public static string menu = "1)Run bot\n2)Check access\n3)Display all pull-requests for repository\n4)Set Current Repository\n5)Get file from repository\n6)Create branch\n7)Change bot run schedule\n8)Logout";
        public static Dictionary<String, Func<Task>> commands = new Dictionary<string, Func<Task>>();
        public static Session s;
        public static OECBot bot;
        public static RepositoryManager rm;

        public static Dictionary<String, IPlugin> plugins = new Dictionary<string, IPlugin>();

        public static void Main(string[] args)
        {
            initalizeCommands();
            Serializer.InitPlugins();
            plugins = Serializer.plugins;
            Console.WriteLine(plugins.Count + " plugins loaded:");
            foreach (IPlugin plugin in getPlugins())
            {
                Console.WriteLine(plugin.GetName() + ": " + plugin.GetDescription());
            }
            while (1 == 1)
            {
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

        }

        public static void initalizeCommands()
        {
            commands.Add("2", CheckAccess);
            commands.Add("1", runBot);
            commands.Add("5", getFile);
            commands.Add("6", createBranch);
            commands.Add("3", getAllPullRequest);
            commands.Add("8", doNothing);
            commands.Add("4", setCurrentRepo);
            commands.Add("7", setSchedule);
            commands.Add("9", forceRun);
        }

        public async static Task forceRun()
        {
            Console.WriteLine("Forcing bot to run now.");
            if (bot.On)
            {
                bot.Stop();
            }
            
            bot.runChecks();
        }

        public async static Task setSchedule()
        {
            
            double hour = 0;
            string hs = "";
            do
            {
                Console.WriteLine("Enter hour of check(int or double):");
                hs = Console.ReadLine();
            } while (!Double.TryParse(hs, out hour));
            double min = 0;
            string ms = "";

            do
            {
                Console.WriteLine("Enter minute of check(int or double):");
                ms = Console.ReadLine();
            } while (!Double.TryParse(ms, out min));
            try
            {
                bot.checkTime = DateTime.Today.AddHours(hour);
                bot.checkTime = bot.checkTime.AddMinutes(min);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Bot set to run at: " + bot.checkTime.ToString());
        }

        public async static Task getAllFiles()
        {
            await rm.getAllFiles("systems/");
        }

        public async static Task doNothing()
        {

        }

        public async static Task setCurrentRepo()
        {
            Console.WriteLine("Enter the name of the owner of the repo:");
            String owner = Console.ReadLine();
            Console.WriteLine("Enter the repository name:");
            String name = Console.ReadLine();
            try
            {
                rm.repo = await s.client.Repository.Get(owner, name);
                bot.rm.repo = rm.repo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Successfully set the current working repository to: {0}/{1}", rm.repo.Owner.Login, rm.repo.Name);
        }

        public async static Task getAllPullRequest()
        {
            try
            {
                var list = await rm.getAllPullRequests();
                Console.WriteLine(list[0].Title);
                foreach (PullRequest pr in list)
                {
                    Console.WriteLine("Pull-Request: {0} - {1}Created by: {2} on {3}, Status: {4}", pr.Title, pr.Body, pr.User.Login, pr.CreatedAt, pr.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async static Task createBranch()
        {
            Console.WriteLine("Enter a name for the new branch!");
            String name = Console.ReadLine();
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
            bot.Start();
        }

        public async static Task getFile()
        {
            Console.WriteLine("Enter the name of the file: ");
            String name = Console.ReadLine();
            try
            {
                String content = await rm.getFile(name);
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
                rm = new RepositoryManager(s, await s.client.Repository.Get("Gazing", "OECTest"));
                bot = new OECBot(getPlugins(), rm.repo);
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
                if (cmd == "8")
                {
                    break;
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

        public async static void AllPullRequests()
        {
            var prs = await rm.getAllPullRequests();
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
