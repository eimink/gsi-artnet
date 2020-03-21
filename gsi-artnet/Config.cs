using System;
using System.IO;
using Newtonsoft.Json;

namespace gsi_artnet
{
    [Serializable]
    class Config
    {
        public bool Debug = false;
        public string ArtNetIP = "127.0.0.1";
        public string ArtNetMask = "255.255.255.0";
        public short ArtNetUniverse = 0;

        public void Load()
        {
            string filePath = Directory.GetCurrentDirectory() + @"\config.json";
            if (File.Exists(filePath))
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(filePath);
                    var fileContents = reader.ReadToEnd();
                    var conf = JsonConvert.DeserializeObject<Config>(fileContents);
                    Debug = conf.Debug;
                    ArtNetIP = conf.ArtNetIP;
                    ArtNetMask = conf.ArtNetMask;
                }
                catch (FileNotFoundException e)
                {
                    
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }
        public void Save()
        {
            TextWriter writer = null;
            try
            {
                writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\config.json", false);
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
    }
}
