using System;
using System.Threading;
using System.Threading.Tasks;
using GeekSyncClient.Helper;
using System.Linq;

namespace GeekSyncClient.Client
{
    public class GenericClient
    {
        private geeksync_server_apiClient client;

        public geeksync_server_apiClient Client {get {return client;}}
        public System.Net.Http.HttpClient httpClient { get; }

        public RSAHelper MyRSA;
        private ConfigManager configManager;

        private bool isAvailable;

        public bool IsAvailable { get { return isAvailable; } }


        public Guid ChannelID{ get; }

        public GenericClient(ConfigManager config,Guid channelID, string baseUrl)
        {
            this.ChannelID = channelID;
            httpClient = new System.Net.Http.HttpClient();
            client = new geeksync_server_apiClient(baseUrl, httpClient);
            this.configManager=config;
            SetupRSA();
        }

        public GenericClient(ConfigManager config,Guid channelID, string baseUrl, System.Net.Http.HttpClient httpClient)
        {
            this.ChannelID = channelID;
            this.httpClient = httpClient;
            client = new geeksync_server_apiClient(baseUrl, httpClient);
            this.configManager=config;
            SetupRSA();
        }

        private void SetupRSA()
        {
            MyRSA=new RSAHelper(configManager.Config.RSAkeys);
            MyRSA.SetPeerPublicKey(configManager.Config.Peers.Single(x=>x.ChannelID==ChannelID).PeerPublicKey);
        }

        public bool CheckIfAvailable()
        {
            bool avail = false;

            try
            {

                var task = Task.Run(async () => { await Client.ChannelAsync(ChannelID); });
                task.Wait();
                avail = true;
            }
            catch
            {
                avail = false;
            }


            isAvailable = avail;
            return avail;
        }

       

        
    }
}