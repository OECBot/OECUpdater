using OECLib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using OECLib.Exoplanets;
using System.Xml;

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


        //public static PlanetSystem loadSystemXML(String filePath)
        //{
        //    PlanetSystem planetSystem;

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(filePath);
        //    XmlNode root = doc.FirstChild;

        //    //Display the contents of the child nodes.
        //    if (root.HasChildNodes)
        //    {
        //        for (int i = 0; i < root.ChildNodes.Count; i++)
        //        {
        //            Console.WriteLine(i + "START: " + root.ChildNodes[i].Name + " :END");

        //            if (root.ChildNodes[i].Name.Equals("star"))
        //            {
        //                for (int j = 0; j < root.ChildNodes[i].ChildNodes.Count; j++)
        //                {
        //                    Console.WriteLine(j + "START: " + root.ChildNodes[i].ChildNodes[j].Name);

        //                    if (root.ChildNodes[i].Name.Equals("planet"))
        //                    {
        //                        for (int j = 0; j < root.ChildNodes[i].ChildNodes.Count; j++)
        //                        {
        //                            Console.WriteLine(j + "START: " + root.ChildNodes[i].ChildNodes[j].Name);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return planetSystem;
        //}
    }
}
