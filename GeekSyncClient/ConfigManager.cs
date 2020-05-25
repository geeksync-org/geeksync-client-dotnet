using System;
using System.Collections.Generic;
using System.IO;
using GeekSyncClient.Config;
using GeekSyncClient.Helper;
using Newtonsoft.Json;

namespace GeekSyncClient
{
    public class ConfigManager
    {

        public ClientConfig Config;
        private string configFileName=".geeksync-client.conf";

        public void ReadConfig()
        {
            FileInfo config = new FileInfo(configFileName);
            StreamReader sr = config.OpenText();
            string configtext = sr.ReadToEnd();
            sr.Close();
            Config = JsonConvert.DeserializeObject<ClientConfig>(configtext);
        }

        public void WriteConfig()
        {
            FileInfo config = new FileInfo(configFileName);
            config.Delete();
            string jsonString;
            jsonString = JsonConvert.SerializeObject(Config);
            StreamWriter sw = config.AppendText();
            sw.WriteLine(jsonString);
            sw.Close();
        }

        public void LoadOrCreateConfig()
        {
            FileInfo config = new FileInfo(configFileName);
            if (config.Exists)
            {
                ReadConfig();
            }
            else
            {
                Config = new ClientConfig();

                RSAHelper tempRSA = new RSAHelper();

                Config.RSAkeys = tempRSA.KeysXml;
                Config.MyID = Guid.NewGuid();
                Config.MyPublicKey = tempRSA.MyPublicKey;

                Peer self = new Peer() { PeerID = Config.MyID, ChannelID = Guid.NewGuid(), PeerPublicKey = tempRSA.MyPublicKey };

                Config.Peers = new List<Peer>();
                Config.Peers.Add(self);
                WriteConfig();
            }
        }

        public ConfigManager()
        {
          LoadOrCreateConfig();
          
        }
        public ConfigManager(string configFile)
        {
            configFileName=configFile;
            LoadOrCreateConfig();
        }

        public void AddPeer(Guid channelID, Guid peerID, string peerPublicKey)
        {
            Peer peer = new Peer() { ChannelID = channelID, PeerID = peerID, PeerPublicKey = peerPublicKey };
            AddPeer(peer);
        }

        public void AddPeer(Peer peer)
        {
            Config.Peers.Add(peer);
            WriteConfig();
        }

        public Guid PeerWith(ConfigManager other)
        {
            Guid channelID=Guid.NewGuid();
            this.AddPeer(channelID,other.Config.MyID,other.Config.MyPublicKey);
            other.AddPeer(channelID,this.Config.MyID, this.Config.MyPublicKey);
            return channelID;
        }

    }
}