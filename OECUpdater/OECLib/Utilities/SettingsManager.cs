using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace OECLib.Utilities
{
    public class SettingsManager
    {
        Dictionary<string, string> settings;
        string path;

        public SettingsManager(string path)
        {
            settings = new Dictionary<string, string>();
            this.path = path;
			if (!File.Exists (path)) {
				File.Create (path);
			}
            string settingsblob = File.ReadAllText(path);
            string line;
            StringReader sr = new StringReader(settingsblob);
            while((line = sr.ReadLine()) != null)
            {
                string[] fields = Regex.Split(line, "=(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                if (fields.Length != 2 || settings.ContainsKey(fields[0]))
                    continue;
                settings.Add(fields[0], fields[1]);
            }
        }

        /// <summary>
        /// Adds a given setting if it doesn't exist already.
        /// </summary>
        /// <param name="key">The setting that needs to be added.</param>
        /// <param name="value">The value of the setting.</param>
        /// <returns>True if the add was successful, false otherwise.</returns>
        public bool AddSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
                return false;
            settings.Add(key, value);
            return true;
        }

        /// <summary>
        /// Changes the value of a given setting if it exists.
        /// </summary>
        /// <param name="key">The setting that needs to be changed.</param>
        /// <param name="value">The value of the setting.</param>
        /// <returns>True if the change was successful, false otherwise.</returns>
        public bool ChangeSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieve a value associated with a setting.
        /// </summary>
        /// <param name="key">The setting name.</param>
        /// <returns>The value associated with the setting if it exists, otherwise null.</returns>
        public string GetSetting(string key)
        {
            if (settings.ContainsKey(key))
                return settings[key];
            return null;
        }

        /// <summary>
        /// Saves the current settings to a file at the path specified when this class was created.
        /// </summary>
        public void SaveSettingsToFile()
        {
            TextWriter newSettings = File.CreateText(path);
            foreach(string key in settings.Keys)
            {
                newSettings.Write(key + "=" + settings[key]);
            }
            newSettings.Flush();
            newSettings.Close();
        }
    }
}
