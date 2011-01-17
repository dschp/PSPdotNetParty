/*
Copyright (C) 2010,2011 monte

This file is part of PSP NetParty.

PSP NetParty is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PspDotNetParty
{
    public class IniParser
    {
        Dictionary<string, Dictionary<string, string>> Table = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> RootSection = new Dictionary<string, string>();

        string _IniFilePath;

        /// <summary>
        /// Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(string iniPath = null)
        {
            if (iniPath == null)
            {
                string appname = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;
                iniPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + appname + ".ini";
            }
            _IniFilePath = iniPath;

            TextReader fileReader = null;
            Dictionary<string, string> currentSection = null;

            if (File.Exists(iniPath))
            {
                try
                {
                    fileReader = new StreamReader(iniPath, Encoding.UTF8);

                    string strLine = fileReader.ReadLine();

                    while (strLine != null)
                    {
                        strLine.Trim();

                        if (strLine == "" || strLine.StartsWith("'")) { }
                        else if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                        {
                            string section = strLine.Substring(1, strLine.Length - 2);
                            if (!Table.ContainsKey(section))
                            {
                                currentSection = Table[section] = new Dictionary<string, string>();
                            }
                            else
                            {
                                currentSection = Table[section];
                            }
                        }
                        else
                        {
                            string[] keyPair = strLine.Split(new char[] { '=' }, 2);

                            if (currentSection == null)
                                currentSection = RootSection;

                            if (keyPair.Length == 2)
                            {
                                string key = keyPair[0].Trim();
                                string value = keyPair[1].Trim();
                                currentSection[key] = value;
                            }
                        }

                        strLine = fileReader.ReadLine();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (fileReader != null)
                        fileReader.Close();
                }
            }
            else
            {
                TextWriter writer = new StreamWriter(iniPath);
                writer.Close();
            }
        }

        Dictionary<string, string> findSection(string sectionName)
        {
            Dictionary<string, string> selectedSection;
            if (sectionName == null)
            {
                selectedSection = RootSection;
            }
            else if (Table.ContainsKey(sectionName))
            {
                selectedSection = Table[sectionName];
            }
            else
            {
                selectedSection = Table[sectionName] = new Dictionary<string, string>();
            }
            return selectedSection;
        }

        /// <summary>
        /// Returns the value for the given section, key pair.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="settingName">Key name.</param>
        public string GetSetting(string sectionName, string settingName, object defaultValue = null)
        {
            Dictionary<string, string> section = findSection(sectionName);

            if (section.ContainsKey(settingName))
                return section[settingName];
            else if (defaultValue != null)
                return section[settingName] = defaultValue.ToString();
            else
                return null;
        }

        /// <summary>
        /// Enumerates all lines for given section.
        /// </summary>
        /// <param name="sectionName">Section to enum.</param>
        public Dictionary<string, Dictionary<string, string>>.KeyCollection EnumSection(string sectionName)
        {
            return Table.Keys;
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void SetSetting(string sectionName, string settingName, string settingValue)
        {
            if (settingName == null)
                throw new ArgumentNullException();

            Dictionary<string, string> selectedSection = findSection(sectionName);
            selectedSection[settingName] = settingValue;
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void SetSetting(string sectionName, string settingName)
        {
            SetSetting(sectionName, settingName, null);
        }

        /// <summary>
        /// Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void RemoveSetting(string sectionName, string settingName)
        {
            if (settingName == null)
                throw new ArgumentNullException();

            Dictionary<string, string> selectedSection = findSection(sectionName);
            selectedSection.Remove(sectionName);
        }

        void AppendSettings(StringBuilder sb, Dictionary<string, string> section)
        {
            foreach (KeyValuePair<string, string> p in section)
            {
                sb.AppendFormat("{0}={1}", p.Key, p.Value).Append(Environment.NewLine);
            }
        }

        /// <summary>
        /// Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(string newFilePath)
        {
            StringBuilder sb = new StringBuilder();

            AppendSettings(sb, RootSection);

            foreach (KeyValuePair<string, Dictionary<string, string>> p in Table)
            {
                sb.AppendFormat("[{0}]", p.Key).Append(Environment.NewLine);
                AppendSettings(sb, p.Value);
                sb.AppendLine();
            }

            try
            {
                TextWriter writer = new StreamWriter(newFilePath, false, Encoding.UTF8);
                writer.Write(sb.ToString());
                writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(_IniFilePath);
        }
    }
}
