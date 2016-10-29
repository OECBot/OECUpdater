using Octokit;
using OECLib.Exoplanets;
using OECLib.GitHub;
using OECLib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECLib
{
    public class OECBot
    {
        public Session session;
        public static String userName = "";
        public static String password = "";

        public List<IPlugin> plugins;

        public bool On;

        public static TimeSpan checkTime = new TimeSpan(12, 0, 0);

        public OECBot(List<IPlugin> plugins)
        {
            this.session = new Session(new Credentials(userName, password));
            this.On = false;
        }

        public async Task Start()
        {
            this.On = true;
            List<Task<List<Planet>>> tasks = new List<Task<List<Planet>>>(); 
            while (On)
            {
                if (checkTime.CompareTo(DateTime.Now.TimeOfDay) >= 0)
                {
                    foreach (IPlugin plugin in plugins)
                    {
                        tasks.Add(runPluginAsync(plugin));
                    }
                }
                foreach (Task<List<Planet>> task in tasks) {
                    List<Planet> newData = await task;
                }
                //Checks

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
