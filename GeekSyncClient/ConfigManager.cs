using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeekSyncClient.Config;
using GeekSyncClient.Helper;

namespace GeekSyncClient
{
    public class ConfigManager
    {

        public ClientConfig Config;

        public ConfigManager(string configFile)
        {
            FileInfo config = new FileInfo(configFile);
            if (!config.Exists)
            {
                //Console.WriteLine("Config file does not exist, creating default.");

                Config = new ClientConfig();

                RSAHelper tempRSA = new RSAHelper();

                Config.RSAkeys = tempRSA.KeysXml;
                Config.MyID = Guid.NewGuid();
                Config.MyPublicKey=tempRSA.MyPublicKey;

                Peer self = new Peer() { PeerID = Config.MyID, ChannelID = Guid.NewGuid(), PeerPublicKey = tempRSA.MyPublicKey };

                Config.Peers = new List<Peer>();
                Config.Peers.Add(self);



                string jsonString;
                jsonString = JsonSerializer.Serialize(Config);
                // Console.WriteLine("-----");
                // Console.WriteLine(jsonString);
                // Console.WriteLine("-----");
                StreamWriter sw = config.AppendText();
                sw.WriteLine(jsonString);
                sw.Close();
            }

            StreamReader sr = config.OpenText();
            string configtext = sr.ReadToEnd();
            sr.Close();
            Config = JsonSerializer.Deserialize<ClientConfig>(configtext);

        }



    }
}