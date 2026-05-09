using System;
using System.IO;
using System.Windows;

namespace DjangoWPFClient.Services
{
    public static class ThemeManager
    {
        private static readonly string ConfigFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DjangoWPFClient", "theme.txt");

        public static string CurrentTheme { get; private set; } = "Light";

        public static void Init()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    var saved = File.ReadAllText(ConfigFile).Trim();
                    if (saved == "Dark" || saved == "Light")
                        CurrentTheme = saved;
                }
            }
            catch { }
            ApplyTheme(CurrentTheme);
        }

        public static void Toggle()
        {
            CurrentTheme = CurrentTheme == "Light" ? "Dark" : "Light";
            ApplyTheme(CurrentTheme);
            Save();
        }

        private static void ApplyTheme(string theme)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"Themes/{theme}.xaml", UriKind.Relative)
            };
            // Заменяем первый MergedDictionary (тему)
            if (Application.Current.Resources.MergedDictionaries.Count > 0)
                Application.Current.Resources.MergedDictionaries[0] = dict;
            else
                Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private static void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(ConfigFile, CurrentTheme);
            }
            catch { }
        }
    }
}