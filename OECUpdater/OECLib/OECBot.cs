using Octokit;
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
            while (On)
            {
                if (checkTime.CompareTo(DateTime.Now.TimeOfDay) == 0)
                {
                    foreach (IPlugin plugin in plugins)
                    {
                        plugin.Run();
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            this.On = false;
        }
    }
}
