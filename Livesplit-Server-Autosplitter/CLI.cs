using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Livesplit_Server_Autosplitter
{
    public class CLI
    {
        public static void HandleInput()
        {
            Console.Write("> ");
            string rawInput = Console.ReadLine();
            Console.WriteLine();

            string[] parts = rawInput.Split(' ');
            string firstWord = parts[0];

            switch (firstWord)
            {
                case "exit":
                case "quit":
                case "q":
                    Environment.Exit(0);
                    break;
                case "settings":
                    SettingsCommand(parts);
                    break;
                case "help":
                    HelpCommand(parts);
                    break;
                default:
                    Console.WriteLine("Unknown command (type help to get the available commands)");
                    break;
            }
        }

        public static void SettingsCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                Console.WriteLine("PLease provide an argument to the function");
                return;
            }
            if (parts[1] == "list")
                ListSettings(ref Program.Runtime.Autosplitter.settings);

            if (parts[1] == "info")
            {
                if(parts.Length < 3)
                {
                    Console.WriteLine("Missing id, type settings list to get a list of the settings with their ids");
                    return;
                }

                bool parsed = int.TryParse(parts[2], out int id);
                if(parsed && id > 0)
                    SettingInfo(ref Program.Runtime.Autosplitter.settings, id);
                else
                {
                    Console.WriteLine("Wrong id, type settings list to get a list of the settings with their ids");
                    return;
                }
            }
            if(parts[1] == "set")
            {
                if(parts.Length < 4)
                {
                    Console.WriteLine("Missing parameters, syntax : settings set (id) (true | false)");
                    return;
                }

                bool parsed = int.TryParse(parts[2], out int id);
                if(!(parsed && id > 0))
                {
                    Console.WriteLine("Wrong id, type settings list to get a list of the settings with their ids");
                    return;
                }

                parsed = bool.TryParse(parts[3], out bool newValue);
                if(parsed)
                    SetSettingCommand(ref Program.Runtime.Autosplitter.settings, id, newValue);
                else
                {
                    Console.WriteLine("Bad value, accepted values are \"true\" and \"false\"");
                    return;
                }
            }
            if(parts[1] == "save")
            {
                if(parts.Length != 3)
                {
                    Console.WriteLine("Syntax : settings save \"path\"");
                    return;
                }

                string path;
                try { path = parts[2].Split('\"')[1]; }
                catch { Console.WriteLine("Bad path given, Syntax : settings save \"path\""); return; }

                SaveSettings(path, ref Program.Runtime.Autosplitter.settings);
            }
            if(parts[1] == "load")
            {
                if(parts.Length != 3)
                {
                    Console.WriteLine("Syntax : settings load \"path\"");
                    return;
                }

                string path;
                try { path = parts[2].Split('\"')[1]; }
                catch { Console.WriteLine("Bad path given, Syntax : settings load \"path\""); return; }

                LoadSettings(path, ref Program.Runtime.Autosplitter.settings);
            }
        }

        public static void ListSettings(ref ASLSettings Settings)
        {
            int WindowWidth = Console.WindowWidth;

            Console.WriteLine("Start : \t" + Program.Runtime.start);
            Console.WriteLine("Split : \t" + Program.Runtime.split);
            Console.WriteLine("Reset : \t" + Program.Runtime.reset);
            Console.WriteLine();

            for (int i = 0; i < Settings.OrderedSettings.Count; i ++)
            {
                string indentation = "";
                if(Settings.OrderedSettings[i].Parent != null)
                {
                    int count = 1;
                    ASLSetting tempSetting = Settings.Settings[Settings.OrderedSettings[i].Parent];
                    do
                    {
                        if(tempSetting.Parent == null)
                            break;
                        tempSetting = Settings.Settings[tempSetting.Parent];
                        count++;
                    }
                    while (true);
                    indentation = new string('\t', count);
                }
                Console.WriteLine(indentation + Settings.OrderedSettings[i].Label + $" ({i + 1}) ({Settings.OrderedSettings[i].Id}) :" + Environment.NewLine + indentation + "   " + Settings.OrderedSettings[i].Value);
            }
        }

        public static void SettingInfo(ref ASLSettings settings, int id)
        {
            if(id > settings.OrderedSettings.Count)
            {
                Console.WriteLine("Wrong id, type settings list to get a list of the settings with their ids");
                return;
            }
            
            ASLSetting setting = settings.OrderedSettings[id - 1];

            string parentString = "None";

            if(setting.Parent != null)
            {
                int count = 1;
                parentString = setting.Parent + " ";
                ASLSetting tempSetting = settings.Settings[setting.Parent];
                do
                {
                    if(tempSetting.Parent == null)
                        break;
                    tempSetting = settings.Settings[tempSetting.Parent];
                    count++;
                    parentString += ", " + tempSetting.Parent;
                }
                while (true);
                parentString += "(" + count + ")";
            }

            Console.WriteLine($"{setting.Label} ({setting.Id}) :" + Environment.NewLine
            + "Tooltip : " + setting.ToolTip + Environment.NewLine
            + "Value : " + setting.Value + Environment.NewLine
            + "Default value : " + setting.DefaultValue + Environment.NewLine
            + "Parent(s) : " + parentString);
        }

        public static void SetSettingCommand(ref ASLSettings settings, int id, bool value)
        {
            if(id > settings.OrderedSettings.Count)
            {
                Console.WriteLine("Wrong id, type settings list to get a list of the settings with their ids");
                return;
            }

            ASLSetting listSetting = settings.OrderedSettings[id - 1];
            ASLSetting dicSetting = settings.Settings[listSetting.Id];

            if(listSetting.Parent == null)
            {
                SetSetting(ref listSetting, ref dicSetting, value);
            }
            else
            {
                Console.WriteLine("This setting has parents, also set them? (y/n)");
                Console.Write("> ");
                ConsoleKey key = Console.ReadKey().Key;
                Console.WriteLine();

                if(key == ConsoleKey.Y)
                {
                    SetSetting(ref listSetting, ref dicSetting, value);

                    ASLSetting tempDicSetting = settings.Settings[listSetting.Parent];
                    ASLSetting tempListSetting = settings.OrderedSettings.Find(x => x.Id == tempDicSetting.Id);

                    SetSetting(ref tempListSetting, ref tempDicSetting, value);

                    do
                    {
                        if(tempDicSetting.Parent == null)
                            break;

                        tempDicSetting = settings.Settings[tempListSetting.Parent];
                        tempListSetting = settings.OrderedSettings.Find(x => x.Id == tempDicSetting.Id);

                        SetSetting(ref tempListSetting, ref tempDicSetting, value);
                    }
                    while (true);

                }
                else if(key == ConsoleKey.N)
                {
                    SetSetting(ref listSetting, ref dicSetting, value);
                }
                else
                {
                    Console.WriteLine("Other key pressed, defaulting to no");
                    SetSetting(ref listSetting, ref dicSetting, value);
                }
            }
        }

        public static void SetSetting(ref ASLSetting listSetting, ref ASLSetting dicSetting, bool value, bool log = true)
        {
            listSetting.Value = value;
            dicSetting.Value = value;

            if(log)
                Console.WriteLine("Set " + listSetting.Id + " to " + value.ToString());
        }

        public static void SaveSettings(string path, ref ASLSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings.Settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void LoadSettings(string path, ref ASLSettings settings)
        {
            string json = File.ReadAllText(path);
            settings.Settings = JsonConvert.DeserializeObject<Dictionary<string, ASLSetting>>(json);

            settings.OrderedSettings.Clear();
            foreach (ASLSetting setting in settings.Settings.Values)
            {
                settings.OrderedSettings.Add(setting);
            }
        }

        public static void HelpCommand(string[] parts)
        {
            Console.WriteLine("exit | quit | q : Exits the program" + Environment.NewLine
                    + "settings (list | info (id) | set (id) (true | false) | save | load) : Manage the settings" + Environment.NewLine
                    + "help : Prints this message");
        }
    }
}