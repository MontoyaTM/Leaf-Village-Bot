using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Config
{
    public class JSONReader
    {
        public string Token {  get; set; }
        public string Prefix { get; set; }

        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public async Task ReadBotConfigJSONAsync()
        {
            using (StreamReader reader = new StreamReader("ConfigBot.json", new UTF8Encoding(false)))
            {
                // Reading Config File
                string json = await reader.ReadToEndAsync();

                // Deserialising file into the ConfigBot struct
                ConfigBot config = JsonConvert.DeserializeObject<ConfigBot>(json);

                this.Token = config.Token;
                this.Prefix = config.Prefix;
            }
        }

        public async Task ReadDBConfigJSONAsync()
        {
            using (StreamReader reader = new StreamReader("ConfigDB.json", new UTF8Encoding(false)))
            {
                string json = await reader.ReadToEndAsync();

                ConfigDB config = JsonConvert.DeserializeObject<ConfigDB>(json);

                this.Host = config.Host;
                this.Username = config.Username;
                this.Password = config.Password;
                this.Database = config.Database;
            }
        }
    }
}
