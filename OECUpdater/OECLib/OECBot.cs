using Octokit;
using OECLib.Exoplanets;
using OECLib.GitHub;
using OECLib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib
{
    public class OECBot
    {
        public Session session;
        public RepositoryManager rm;
        public static String userName = "OECBot";
        public static String password = "UoJ84XJTXphgO4F";
        private CancellationTokenSource cts;

        public List<IPlugin> plugins;

        public bool On;

        public static DateTime checkTime = DateTime.Today.AddHours(15);

        public OECBot(List<IPlugin> plugins, Repository repo)
        {
            this.session = new Session(new Credentials(userName, password));
            this.rm = new RepositoryManager(session, repo);
            this.plugins = plugins;
            this.On = false;
        }

        public void Start()
        {
            this.On = true;
            this.cts = new CancellationTokenSource();
            checkTime = checkTime.AddMinutes(33);
            if (checkTime < DateTime.Now)
            {
                checkTime = checkTime.AddDays(1.0);
            }
            scheduleCheck(runChecks);
            Console.WriteLine("Bot will perform check in: {0}", checkTime - DateTime.Now);
            
        }

        private async void scheduleCheck(Func<CancellationToken, Task> check)
        {
            await Task.Delay((int)checkTime.Subtract(DateTime.Now).TotalMilliseconds);
            try
            {
                await check(cts.Token);
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("OECBot stopped..."+oce.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async Task runChecks(CancellationToken token)
        {
            List<Task<List<Planet>>> tasks = new List<Task<List<Planet>>>();
            List<Planet> newData = new List<Planet>();

            checkTime = checkTime.AddDays(1.0);
            scheduleCheck(runChecks);

            foreach (IPlugin plugin in plugins)
            {
                Console.WriteLine("Running plugin: {0}", plugin.GetName());
                tasks.Add(runPluginAsync(plugin));
            }

            foreach (Task<List<Planet>> task in tasks)
            {
                token.ThrowIfCancellationRequested();
                List<Planet> planets = await task;
                newData.AddRange(planets);
            }

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

        private async Task<List<Planet>> runPluginAsync(IPlugin plugin) {
            return plugin.Run();
        }

        public void Stop()
        {
            this.On = false;
            cts.Cancel();
            cts.Dispose();
        }
    }
}
