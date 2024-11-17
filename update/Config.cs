using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace update
{
    public enum UpdateType
    {
        Zip,
        File
    }
    public class Config
    {
        public  string bucketName { get; set; } = "";
        public  string endpoint { get; set; } = "";
        public  string launcherName { get; set; } = "";
        public string updateType { get; set; } = "zip";

        private static Config _instance = new Config();

        public static Config Instance => _instance;
        
        public static void Load()
        {
            try
            {
                if (File.Exists("config.json"))
                {
                    _instance = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

                }else
                {
                    _instance = new Config();
                }

            }
            catch
            {
                _instance = new Config();
            }
        }
        public static void Save()
        {
            File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(_instance));
        }
    }
}
