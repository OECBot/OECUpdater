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
using OECLib.Exoplanets.Units;

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

		public static void writeToXML(string fileName){

		}

        public static PlanetSystem LoadXMLFile(string fileName)
        {
            PlanetSystem system = null;
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode root = doc.FirstChild;
            
            if (root.HasChildNodes)
            {
               system = getSystemFromNode(root);
            }

            return system;
        }

        private static PlanetSystem getSystemFromNode(XmlNode systemNode)
        {
            List<Star> xmlStars = new List<Star>();
            PlanetSystem newSystem = new PlanetSystem(xmlStars);
            foreach (XmlNode systemElement in systemNode)
            {
                if (systemElement.Name.Equals("star"))
                {
                    Star newStar = getStarFromNode(systemElement);
                    xmlStars.Add(newStar);
                }
                else
                {
                    String errorMinus = "0.0", errorPlus = "0.0";
                    foreach (XmlAttribute attribute in systemElement.Attributes)
                    {
                        switch (attribute.Name)
                        {
                            case "errorminus":
                                errorMinus = attribute.InnerText;
                                break;
                            case "errorplus":
                                errorPlus = attribute.InnerText;
                                break;
                        }
                    }
                    UnitError element = new UnitError(systemElement.Name, systemElement.InnerText,
                        Double.Parse(errorMinus), Double.Parse(errorPlus));

                    newSystem.addElement(element);
                }
            }

            return newSystem;
        }

        private static Star getStarFromNode(XmlNode starNode)
        {
            List<Planet> xmlPlanets = new List<Planet>();
            Star newStar = new Star(xmlPlanets);
            foreach (XmlNode starElement in starNode.ChildNodes)
            {
                if (starElement.Name.Equals("planet"))
                {
                    Planet newPlanet = getPlanetFromNode(starElement);
                    xmlPlanets.Add(newPlanet);
                }
                else
                {
                    String errorMinus = "0.0", errorPlus = "0.0";
                    foreach (XmlAttribute attribute in starElement.Attributes)
                    {
                        switch (attribute.Name)
                        {
                            case "errorminus":
                                errorMinus = attribute.InnerText;
                                break;
                            case "errorplus":
                                errorPlus = attribute.InnerText;
                                break;
                        }
                    }
                    UnitError element = new UnitError(starElement.Name, starElement.InnerText,
                        Double.Parse(errorMinus), Double.Parse(errorPlus));
                    newStar.addElement(element);
                }
            }

            return newStar;
        }

        private static Planet getPlanetFromNode(XmlNode planetNode)
        {
            Planet newPlanet = new Planet();

            foreach (XmlNode planetElement in planetNode.ChildNodes)
            {
                String errorMinus = "0.0", errorPlus = "0.0";
                foreach (XmlAttribute attribute in planetElement.Attributes)
                {
                    switch (attribute.Name)
                    {
                        case "errorminus":
                            errorMinus = attribute.InnerText;
                            break;
                        case "errorplus":
                            errorPlus = attribute.InnerText;
                            break;
                    }
                }
                UnitError element = new UnitError(planetElement.Name, planetElement.InnerText,
                    Double.Parse(errorMinus), Double.Parse(errorPlus));
                newPlanet.addElement(element);
            }

            return newPlanet;
        }
    }
}
