using Octokit;
using OECLib.Data;
using OECLib.GitHub;
using OECLib.Interface;
using OECLib.Utilities;
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
        public const String userName = "OECBot";
        public const String password = "UoJ84XJTXphgO4F";
        private CancellationTokenSource cts;
        public int Workers = 10;
        public bool isFirstRun = true;
        public Object queueLock = new Object();
        public Queue<RepositoryContent> fileQueue;

        public List<IPlugin> plugins;

        public bool On;

        public DateTime checkTime = DateTime.Today.AddHours(15);
        public DateTime lastCheckTime = DateTime.MinValue;

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
            if (isFirstRun)
            {
                scheduleCheck(firstRun);
            }
            else
            {
                scheduleCheck(runChecks);
            }
            Console.WriteLine("Bot will perform check in: {0}", checkTime - DateTime.Now);

        }

        private async void scheduleCheck(Func<CancellationToken, Task> check)
        {
            await Task.Delay((int)checkTime.Subtract(DateTime.Now).TotalMilliseconds);

            try
            {
                lastCheckTime = DateTime.Now;
                checkTime = checkTime.AddDays(1.0);

                scheduleCheck(runChecks);
                await check(cts.Token);
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("OECBot stopped..." + oce.Message);
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

        public async Task firstRun(CancellationToken token)
        {
            var files = await rm.getAllFiles("systems/");
            Task[] tasks = new Task[10];
            fileQueue =  new Queue<RepositoryContent>();
            foreach (RepositoryContent file in files)
            {
                fileQueue.Enqueue(file);
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = runWorker();
            }
            foreach (Task task in tasks)
            {
                await task;
            }
        }

        private RepositoryContent dequeueFile()
        {
            lock (queueLock)
            {
                var file = this.fileQueue.Dequeue();
                return file;
            }
            
        }

        public async Task runWorker()
        {
            while (fileQueue.Count != 0)
            {
                var file = this.dequeueFile();
                try
                {
                    String content = await rm.getFile("systems/" + file.Name);
                    String systemName = file.Name.Split('.')[0];
                    XMLDeserializer xmld = new XMLDeserializer(content, false);
                    StellarObject system = xmld.ParseXML();
                    String lastUpdate = "";

                    foreach (IPlugin plugin in plugins)
                    {
                        List<StellarObject> systems = plugin.Run(lastUpdate, systemName);

                    }
                    //Merge all of them!
                    String newContent = "";
                    await rm.BeginCommitAndPush("systems/" + file.Name, newContent, "", false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to update " + file.Name+": "+ex.Message);
                }
            }
        }

        private async Task<List<StellarObject>> runPluginAsync(IPlugin plugin) {
            return plugin.Run(lastCheckTime.ToString("yyyy-mm-dd"));
        }

        public void Stop()
        {
            this.On = false;
            cts.Cancel();
            cts.Dispose();
        }
    }
}
