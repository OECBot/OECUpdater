using OECLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using OECLib.Data;

namespace OECLib.Utilities
{
    public static class Serializer
    {
        public static Dictionary<String, IPlugin> plugins = new Dictionary<String, IPlugin>();

        public static void InitPlugins()
        {
            string[] files = Directory.GetFiles(Application.StartupPath + "/Plugins/", "*.dll", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string fname = files[i];
                Assembly assembly = Assembly.LoadFrom(fname);
                //Console.WriteLine(fname);
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    bool flag3 = type.IsPublic && !type.IsAbstract;
                    if (flag3)
                    {
                        try
                        {
                            Type @interface = type.GetInterface("OECLib.Interface.IPlugin");
                            bool flag4 = @interface != null;
                            if (flag4)
                            {
                                AttachPlugin(type);
                               
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to load plugin: " + ex.Message);
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


		public static void writeToXML(string fileName, SolarSystem system){
			StringBuilder output = new StringBuilder();
			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Indent = true;
			ws.OmitXmlDeclaration = true;

			using (XmlWriter xw = XmlWriter.Create(fileName, ws))
			{
				system.Write (xw);
				xw.Flush();
			}

//			Console.WriteLine(output.ToString());

		}

	}
}
