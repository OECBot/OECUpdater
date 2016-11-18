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

namespace OECLib
{
    public class OECBot
    {
        public Session session;
        public RepositoryManager rm;
        public const String userName = "OECBot";
        public const String password = "UoJ84XJTXphgO4F";
        private CancellationTokenSource cts;
        public int Workers = 1;
        public bool isFirstRun = false;
        public Object queueLock = new Object();
        public Queue<RepositoryContent> fileQueue;

        public List<IPlugin> plugins;

        public bool On;

        public DateTime checkTime = DateTime.Today.AddHours(12);
        public DateTime lastCheckTime = DateTime.ParseExact("2016-09-01", "yyyy-MM-dd", null);
        public DateTime tempTime;

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
            

        }

        private async void scheduleCheck(Func<CancellationToken, Task> check)
        {
            Console.WriteLine("Bot will perform check in: {0}", checkTime - DateTime.Now);
            await Task.Delay(1);
            //(int)checkTime.Subtract(DateTime.Now).TotalMilliseconds

            try
            {
                tempTime = DateTime.Now;
                checkTime = checkTime.AddDays(1.0);

                //scheduleCheck(runChecks);
                await check(cts.Token);
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("OECBot stopped..." + oce.Message);
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
            
            while (newData.Count != 0)
            {
                StellarObject update = newData[0];
                try
                {
                    bool isNew = false;
                    if (update.names[0].MeasurementValue == null)
                    {
                        throw new Exception("System name is empty. Skipping...");
                    }
                    Task<String> xmlTask = rm.getFile("systems/" + update.names[0].MeasurementValue + ".xml");
                    List<StellarObject> system = newData.FindAll(x => x.names[0].MeasurementValue == update.names[0].MeasurementValue);
                    newData.RemoveAll(x => x.names[0].MeasurementValue == update.names[0].MeasurementValue);
                    List<String> lastUpdates = new List<string>();

                    String xml = await xmlTask;
                    String updatedXml;
                    if (xml != null)
                    {
                        Console.WriteLine("Found existing system: {0} in OEC. Proceed with update.", update.names[0].MeasurementValue);
                        XMLDeserializer xmld = new XMLDeserializer(xml, false);
                        StellarObject original = xmld.ParseXML();
                        if (!needUpdate(original.getLastUpdate(), system))
                        {
                            continue;
                        }
                        foreach (StellarObject planet in system)
                        {
                            PlanetMerger.Merge(planet, original);
                        }
                        StringBuilder output = new StringBuilder();
                        XmlWriterSettings ws = new XmlWriterSettings();
                        ws.Indent = true;
                        ws.OmitXmlDeclaration = true;
                        using (XmlWriter xw = XmlWriter.Create(output, ws))
                        {
                            original.Write(xw);
                            xw.Flush();
                        }
                        updatedXml = output.ToString();
                        output.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Could not find existing system: {0} in OEC. Proceed with addition.", update.names[0].MeasurementValue);
                        isNew = true;
                        StellarObject baseSys = system[0];
                        system.Remove(baseSys);
                        foreach (StellarObject otherPlanet in system)
                        {
                            PlanetMerger.Merge(otherPlanet, baseSys);
                        }
                        StringBuilder output = new StringBuilder();
                        XmlWriterSettings ws = new XmlWriterSettings();
                        ws.Indent = true;
                        ws.OmitXmlDeclaration = true;
                        using (XmlWriter xw = XmlWriter.Create(output, ws))
                        {
                            baseSys.Write(xw);
                            xw.Flush();
                        }
                        updatedXml = output.ToString();
                        output.Clear();
                    }
                    await rm.BeginCommitAndPush("systems/" + update.names[0].MeasurementValue + ".xml", updatedXml, "N/A", isNew);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to update planet: " + update.names[0].MeasurementValue + "-" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                Console.WriteLine("Successfully commited data for: {0} ", update.names[0].MeasurementValue);
            }
            
            lastCheckTime = tempTime;
            
        }

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

        private async Task<List<StellarObject>> runPluginAsync(IPlugin plugin) {
            return plugin.Run(lastCheckTime.ToString("yyyy-MM-dd"));
        }

        public void Stop()
        {
            this.On = false;
            cts.Cancel();
            cts.Dispose();
        }
    }
}
