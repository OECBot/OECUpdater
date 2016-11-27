using Octokit;
using OECLib.Data;
using OECLib.GitHub;
using OECLib.Interface;
using OECLib.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Concurrent;

namespace OECLib
{
    public class OECBot
    {
        public Session session;
        public RepositoryManager rm;
        public const String userName = "OECBot";
        public const String password = "UoJ84XJTXphgO4F";
        private CancellationTokenSource cts;
        private CancellationToken token;

        public int Workers = 1;
        public bool isFirstRun = false;
		public bool isRunning = false;
        public Object updateLock = new Object();
        public List<StellarObject> updateList;
        public ConcurrentQueue<StellarObject> commitQueue;
        private int finished;
		public int updateCount { get; private set; }
		public int total { get; private set; }
		public int updatesFound { get; private set; }
		public int updatesLeft { get; private set; }
		public String lastRunCondition;
		private int errorCount;

        public List<IPlugin> plugins;

        public bool On;

        public DateTime checkTime = DateTime.Today.AddHours(12);
        public DateTime lastCheckTime = DateTime.MinValue;
        public DateTime tempTime;

		public delegate void UpdateDelegate();
		private UpdateDelegate updateDelegate;

        public OECBot(List<IPlugin> plugins, Repository repo)
        {
            this.session = new Session(new Credentials(userName, password));
            this.rm = new RepositoryManager(session, repo);
            this.plugins = plugins;
            this.On = false;
			//this.cts = new CancellationTokenSource();
			//this.token = cts.Token;
            DateTime.TryParse("2016-09-01", out lastCheckTime);
        }

        public async Task Start()
        {
            this.On = true;
            
			if (updateDelegate != null) {
				this.updateDelegate();
			}
            if (checkTime < DateTime.Now)
            {
                checkTime = checkTime.AddDays(1.0);
            }
            if (isFirstRun)
            {
                lastCheckTime = DateTime.MinValue;
            }
            await scheduleCheck(runChecks);


        }

        private async Task scheduleCheck(Func<Task> check)
        {
			token = cts.Token;
            Logger.WriteLine("Bot scheduled to run in {0}", checkTime - DateTime.Now);
            Console.WriteLine("Bot will perform check in: {0}", checkTime - DateTime.Now);
            await Task.Delay((int)checkTime.Subtract(DateTime.Now).TotalMilliseconds, token);

			cts = new CancellationTokenSource ();
            try
            {
                tempTime = DateTime.Now;
                checkTime = checkTime.AddDays(1.0);

                scheduleCheck(runChecks);
                await check();
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("OECBot stopped..." + oce.Message);
                Logger.WriteWarning("OECBot stopped..." + oce.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

        }

        public bool needUpdate(List<String> original, List<StellarObject> newUpdates)
        {
            List<String> lastUpdates = new List<String>();
            foreach (StellarObject planet in newUpdates)
            {
                List<String> lu = planet.getLastUpdate();
                if (lu != null)
                {
                    lastUpdates.AddRange(lu);
                }

            }
            foreach (String last in original)
            {
                if (!lastUpdates.Contains(last))
                {
                    return true;
                }
            }
            return false;
        }

		public void forceRun()
		{
			this.On = true;
			cts = new CancellationTokenSource ();
			token = cts.Token;

			Task run = runChecks ();
			run.Wait ();
		}

        public async Task runChecks()
        {
			token.ThrowIfCancellationRequested();
			Logger.Initialize ();
            List<Task<List<StellarObject>>> tasks = new List<Task<List<StellarObject>>>();

            List<StellarObject> newData = new List<StellarObject>();

			isRunning = true;
			if (updateDelegate != null) {
				this.updateDelegate();
			}
            DateTime oecStart = DateTime.Now;
            Logger.WriteLine("Begin running plugins.");
            DateTime start = DateTime.Now;
            updateCount = 0;
            total = 0;
            foreach (IPlugin plugin in plugins)
            {
                Console.WriteLine("Running plugin: {0}", plugin.GetName());
                Logger.WriteLine("Running plugin: {0}", plugin.GetName());
				tasks.Add(Task<List<StellarObject>>.Run(() => plugin.Run(lastCheckTime.ToString("yyyy-MM-dd"))));

            }


            foreach (Task<List<StellarObject>> task in tasks)
            {
                token.ThrowIfCancellationRequested();
                List<StellarObject> planets = await task;
                newData.AddRange(planets);
            }
			updatesFound = newData.Count;
			updatesLeft = newData.Count;
            Console.WriteLine("Finished running plugins in: {0} seconds", (DateTime.Now - start).TotalSeconds);
            finished = 0;

            commitQueue = new ConcurrentQueue<StellarObject>();
            updateList = newData;
            Task[] workers = new Task[Workers];
            Console.WriteLine("Starting workers");
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = updateWorker();
            }
            int limit = 0;
            while (finished != Workers)
            {
                while (!commitQueue.IsEmpty)
                {
                    StellarObject update;
                    while (!commitQueue.TryDequeue(out update) && finished != Workers)
                    {
						token.ThrowIfCancellationRequested();
                    }
                    try
                    {
                        total++;
                        StringBuilder output = new StringBuilder();
                        XmlWriterSettings ws = new XmlWriterSettings();
                        ws.Indent = true;
                        ws.OmitXmlDeclaration = true;
                        using (XmlWriter xw = XmlWriter.Create(output, ws))
                        {
                            update.Write(xw);
                            xw.Flush();
                        }
                        String newContent = output.ToString();
                        output.Clear();

                        await rm.BeginCommitAndPush("systems/" + update.names[0].MeasurementValue + ".xml", newContent, update.Source, update.isNew);
						updatesLeft = updateList.Count;
                        updateCount++;
                        limit++;
						if (updateDelegate != null) {
							this.updateDelegate();
						}

                        Console.WriteLine("Successfully commited data for: {0} ", update.names[0].MeasurementValue);
                        Logger.WriteLine("Successfully commited data for: {0} ", update.names[0].MeasurementValue);
						token.ThrowIfCancellationRequested();
                        //60 pushes / min?
                        if (limit >= 35)
                        {
                            Logger.WriteLine("GitHub push limit reached awaiting 60s.");
                            await Task.Delay(60000, token);
                            limit = 0;
                        }

                    }
                    catch (ForbiddenException fex)
                    {
						Console.WriteLine("Triggered abuse mechanism? Sleeping for 60s. " + fex.Message);
                        Logger.WriteWarning("Triggered abuse mechanism? Sleeping for 60s. " + fex.Message);
						if (errorCount >= 5) {
							errorCount = 0;
							lastRunCondition = "Error: Abuse Mechanism";
							return;
						}
						await Task.Delay (60000, token);
						commitQueue.Enqueue (update);
						limit = 0;
						errorCount++;
                    }
					catch (OperationCanceledException ex)
					{
						Console.WriteLine ("Bot cancelled");
						return;
					}
					catch (ApiValidationException ex)
					{
						Console.WriteLine("Failed to update planet: " + update.names[0].MeasurementValue + ". " + ex.Message + ex.GetType());
						Console.WriteLine(ex.HttpResponse.Body);
						Logger.WriteError("Failed to update planet: " + update.names[0].MeasurementValue + ". " + ex.Message + ex.GetType());
						Logger.WriteError(ex.HttpResponse.Body.ToString());
					}
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to update planet: " + update.names[0].MeasurementValue + ". " + ex.Message + ex.GetType());
                        Console.WriteLine(ex.StackTrace);
                        Logger.WriteError("Failed to update planet: " + update.names[0].MeasurementValue + ". " + ex.Message + ex.GetType());
                        Logger.WriteError(ex.StackTrace);
                    }

                }
            }
            Console.WriteLine("Finished running!");
			lastRunCondition = "Finished";
            Logger.WriteLine("Finished running OECBot in {0} seconds was {1}first run.", (DateTime.Now - oecStart).TotalSeconds, isFirstRun ? "" : "not ");
            Logger.WriteLine("Successfully updated {0} systems out of {1} system updates found.", updateCount, total);
			isRunning = false;
			if (updateDelegate != null) {
				this.updateDelegate();
			}
            if (isFirstRun)
            {
                isFirstRun = false;
            }
            lastCheckTime = tempTime;
        }
			
        public async Task updateWorker()
        {
            while (updateList.Count != 0)
            {
				token.ThrowIfCancellationRequested ();
                List<StellarObject> system = dequeueUpdate();
                StellarObject update = system[0];

                Task<String> xmlTask = rm.getFile("systems/" + update.names[0].MeasurementValue + ".xml");
                
                    List<String> lastUpdates = new List<string>();
					token.ThrowIfCancellationRequested();
				try
				{
                    String xml = await xmlTask;
                    if (xml != null)
                    {
                        Console.WriteLine("Found existing system: {0} in OEC. Proceed with update.", update.names[0].MeasurementValue);
						Logger.WriteLine("Found existing system: {0} in OEC. Proceed with update.", update.names[0].MeasurementValue);
                        XMLDeserializer xmld = new XMLDeserializer(xml, false);
                        StellarObject original = xmld.ParseXML();
                        if (!needUpdate(original.getLastUpdate(), system))
                        {
                            Console.WriteLine("Appears that system: {0} does not to be updated continuing.", update.names[0].MeasurementValue);
							Logger.WriteLine("Appears that system: {0} does not to be updated continuing.", update.names[0].MeasurementValue);
                            continue;
                        }
                        String sources = "";
                        foreach (StellarObject planet in system)
                        {
                            PlanetMerger.Merge(planet, original);
                            sources += String.Format("[{0}]({1})\n", planet.children[0].names[0].MeasurementValue, planet.Source);
                        }
                        original.isNew = false;
                        original.Source = sources;
                        commitQueue.Enqueue(original);
                    }
                    else
                    {
                        Console.WriteLine("Could not find existing system: {0} in OEC. Proceed with addition.", update.names[0].MeasurementValue);
						Logger.WriteLine("Could not find existing system: {0} in OEC. Proceed with addition.", update.names[0].MeasurementValue);
						StellarObject baseSys = system[0];
                        system.Remove(baseSys);
                        String sources = "";
						sources += String.Format("[{0}]({1})\n", baseSys.children[0].names[0].MeasurementValue, baseSys.Source);
                        foreach (StellarObject otherPlanet in system)
                        {
                            PlanetMerger.Merge(otherPlanet, baseSys);
                            sources += String.Format("[{0}]({1})\n", otherPlanet.children[0].names[0].MeasurementValue, otherPlanet.Source);
                        }
                        baseSys.isNew = true;
                        baseSys.Source = sources;
                        commitQueue.Enqueue(baseSys);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to create update: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
					Logger.WriteLine("Failed to create update: " + ex.Message);
					Logger.WriteLine(ex.StackTrace);
                }
            }
            Interlocked.Add(ref finished, 1);
        }

		public void setUpdateDelegate(UpdateDelegate updateDelegate) {
			this.updateDelegate = updateDelegate;
		}

        /*
        public async Task firstRun(CancellationToken token)
        {
            var files = await rm.getAllFiles("systems/");
            Task[] tasks = new Task[Workers];
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
         */

        private List<StellarObject> dequeueUpdate()
        {
            lock (updateLock)
            {
                var first = this.updateList[0];
                if (first.names[0].MeasurementValue == null)
                {
                    throw new Exception("System name is empty. Skipping...");
                }
                List<StellarObject> system = updateList.FindAll(x => x.names[0].MeasurementValue == first.names[0].MeasurementValue);
                updateList.RemoveAll(x => x.names[0].MeasurementValue == first.names[0].MeasurementValue);
                return system;
            }

        }

        /*
        public async Task runWorker()
        {
            while (fileQueue.Count != 0)
            {
                var file = this.dequeueFile();
                if (file == null)
                {
                    Console.WriteLine("dequeueFile() return null. Empty queue?");
                    break;
                }
                try
                {
                    String content = await rm.getFile("systems/" + file.Name);
                    String systemName = file.Name.Split('.')[0];
                    XMLDeserializer xmld = new XMLDeserializer(content, false);
                    StellarObject system = xmld.ParseXML();
                    DateTime parsedDate;
                    List<String> updateDates = system.getLastUpdate();
                    updateDates.Sort();
                    String sUpdate = updateDates[0];
                    if (!DateTime.TryParseExact(sUpdate, "yy/MM/dd", null, DateTimeStyles.None, out parsedDate))
                    {
                        Console.WriteLine("Failed to parse lastupdate for file: " + file.Name);
                        continue;
                    }
                    String lastUpdate = parsedDate.ToString("yyyy-MM-dd");
                    List<StellarObject> systems = new List<StellarObject>();
                    foreach (IPlugin plugin in plugins)
                    {
                        List<StellarObject> newStuff = plugin.Run(lastUpdate, systemName);
                        if (newStuff.Count != 0)
                        {
                            systems.AddRange(newStuff);
                        }
                        
                    }
                    if (systems.Count == 0)
                    {
                        Console.WriteLine("Could not find any updates for system: " + file.Name);
                        continue;
                    }
                    foreach (StellarObject update in systems) {
                        PlanetMerger.Merge(update, system);
                    }
                    StringBuilder output = new StringBuilder();
                    XmlWriterSettings ws = new XmlWriterSettings();
                    ws.Indent = true;
                    ws.OmitXmlDeclaration = true;
                    using (XmlWriter xw = XmlWriter.Create(output, ws))
                    {
                        system.Write(xw);
                        xw.Flush();
                    }
                    String newContent = output.ToString();
                    output.Clear();
                    await rm.BeginCommitAndPush("systems/" + file.Name, newContent, "N/A", false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to update " + file.Name);
                    Console.WriteLine(ex.Message+ex.StackTrace);
                    continue;
                }
                Console.WriteLine("Updated " + file.Name + " successfully");
            }
        }
         */

        private async Task<List<StellarObject>> runPluginAsync(IPlugin plugin)
        {
            return plugin.Run(lastCheckTime.ToString("yyyy-MM-dd"));
        }

        public void Stop()
        {
            this.On = false;
			this.lastRunCondition = "Cancelled";

            cts.Cancel();
            cts.Dispose();
        }
    }
}
