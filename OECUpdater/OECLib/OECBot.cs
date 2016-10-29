using Octokit;
using OECLib.Exoplanets;
using OECLib.GitHub;
using OECLib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib
{
    public class OECBot
    {
        public Session session;
        public RepositoryManager rm;
        public static String userName = "";
        public static String password = "";

        public List<IPlugin> plugins;

        public bool On;

        public static DateTime checkTime = DateTime.Today.AddDays(1.0);

        public OECBot(List<IPlugin> plugins, Repository repo)
        {
            this.session = new Session(new Credentials(userName, password));
            this.rm = new RepositoryManager(session, repo);
            this.plugins = plugins;
            this.On = false;
        }

        public async void Start()
        {
            this.On = true;
            List<Task<List<Planet>>> tasks = new List<Task<List<Planet>>>();
            List<Planet> newData = new List<Planet>();
            Console.WriteLine("Running plugin: {0}", plugins[0].GetName());
            while (On)
            {
                Console.WriteLine("Bot will perform check in: {0}", checkTime - DateTime.Now);
                if (checkTime < DateTime.Now)
                {
                    checkTime = checkTime.AddDays(1.0);
                    foreach (IPlugin plugin in plugins)
                    {
                        Console.WriteLine("Running plugin: {0}", plugin.GetName());
                        tasks.Add(runPluginAsync(plugin));
                    }
                    foreach (Task<List<Planet>> task in tasks)
                    {
                        newData.AddRange(await task);
                    }
                    //Checks
                    StringBuilder output = new StringBuilder();
                    XmlWriterSettings ws = new XmlWriterSettings();
                    ws.Indent = true;
                    ws.OmitXmlDeclaration = true;
                    foreach (Planet planet in newData)
                    {
                        using (XmlWriter xw = XmlWriter.Create(output, ws))
                        {
                            planet.Write(xw);
                            xw.Flush();
                        }
                        
                        Console.WriteLine(output.ToString());
                        output.Clear();
                    }
                    newData.Clear();
                }

                System.Threading.Thread.Sleep(50);
            }
        }

        private async Task<List<Planet>> runPluginAsync(IPlugin plugin) {
            return plugin.Run();
        }

        public void Stop()
        {
            this.On = false;
        }
    }
}
