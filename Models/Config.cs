using Newtonsoft.Json;
using System;
using System.IO;

namespace MusicProcessor.Models
{
    public class Config
    {
        private static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ConfigPath = Path.Combine(Config.BaseDir, "config.json");

        public string LibraryFolder { get; set; }
        public string FreqProblemFile { get; set; }
        public string SaveProgramFile { get; set; }
        public int SampleLength { get; set; }

        public static Config Read()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    return JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
                }
            }
            catch (Exception)
            {
                // no code
            }

            return new Config();
        }

        public void Save()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this));
        }
    }
}
