using Octokit;
using OECLib.Data;
using OECLib.GitHub;
using OECLib.Interface;
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
        public int Workers = 10;

        public List<IPlugin> plugins;

        public bool On;

        public DateTime checkTime = DateTime.Today.AddHours(15);

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
			List<Task<List<StellarObject>>> tasks = new List<Task<List<StellarObject>>>();
			List<StellarObject> newData = new List<StellarObject>();

            checkTime = checkTime.AddDays(1.0);
            scheduleCheck(runChecks);

            foreach (IPlugin plugin in plugins)
            {
                Console.WriteLine("Running plugin: {0}", plugin.GetName());
                tasks.Add(runPluginAsync(plugin));
            }

			foreach (Task<List<StellarObject>> task in tasks)
            {
                token.ThrowIfCancellationRequested();
				List<StellarObject> planets = await task;
                newData.AddRange(planets);
            }

            StringBuilder output = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.OmitXmlDeclaration = true;
            foreach (StellarObject planet in newData)
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

        public async Task firstRun()
        {
            var files = await rm.getAllFiles("systems/");
            Task[] tasks = new Task[10];
            Queue<RepositoryContent> queue = new Queue<RepositoryContent>();
            foreach (RepositoryContent file in files)
            {
                queue.Enqueue(file);
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = runWorker(queue);
            }
        }

        public async Task runWorker(Queue<RepositoryContent> files)
        {
            while (files.Count != 0)
            {
                var file = files.Dequeue();
                String content = await rm.getFile("systems/"+file.Name);
                String systemName = file.Name.Split('.')[0];

            }
        }

        private async Task<List<StellarObject>> runPluginAsync(IPlugin plugin) {
            return plugin.Run("2016-09-01");
        }

        public void Stop()
        {
            this.On = false;
            cts.Cancel();
            cts.Dispose();
        }
    }
}
