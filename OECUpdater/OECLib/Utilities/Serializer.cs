using OECLib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace OECLib.Utilities
{
    public static class Serializer
    {
        public static Dictionary<String, IPlugin> plugins = new Dictionary<String, IPlugin>();

        public static void InitPlugins(String appPath)
        {
            string[] files = Directory.GetFiles(appPath + "/Plugins/", "*.dll", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string fname = files[i];
                Assembly assembly = Assembly.LoadFrom(fname);
                Console.WriteLine(fname);
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    bool flag3 = type.IsPublic && !type.IsAbstract;
                    if (flag3)
                    {
                        try
                        {
                            Type @interface = type.GetInterface("OECLib.Plugins.IPlugin");
                            bool flag4 = @interface != null;
                            if (flag4)
                            {
                                AttachPlugin(type);
                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
            }
        }

        private static void AttachPlugin(Type type)
        {
            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
            string name = plugin.GetName();
            plugin.Initialize();
            plugins.Add(name, plugin);
        }
    }
}
