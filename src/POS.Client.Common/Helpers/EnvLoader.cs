using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace POS.Client.Common.Helpers
{
    public class EnvLoader
    {
        private static Dictionary<string, string> _config = new Dictionary<string, string>();
        private static bool _loaded = false;

        public static void Load()
        {
            if (_loaded) return;

            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = FindEnvFile(currentDir);
            
            if (path != null)
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

                    int idx = trimmed.IndexOf('=');
                    if (idx > 0)
                    {
                        string key = trimmed.Substring(0, idx).Trim();
                        string val = trimmed.Substring(idx + 1).Trim();
                        if (!_config.ContainsKey(key))
                        {
                            _config[key] = val;
                        }
                    }
                }
            }
            _loaded = true;
        }

        private static string FindEnvFile(string startDir)
        {
            DirectoryInfo dir = new DirectoryInfo(startDir);
            for (int i = 0; i < 6; i++) 
            {
                string path = Path.Combine(dir.FullName, ".env");
                if (File.Exists(path)) return path;
                dir = dir.Parent;
                if (dir == null) break;
            }
            return null;
        }

        public static string Get(string key, string defaultValue = "")
        {
            if (!_loaded) Load();
            return _config.ContainsKey(key) ? _config[key] : defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            string val = Get(key);
            if (int.TryParse(val, out int result)) return result;
            return defaultValue;
        }
    }
}
