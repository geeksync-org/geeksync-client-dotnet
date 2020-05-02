using System;

namespace GeekSyncClient.Client
{
    public class GenericClient
    {
        private geeksync_server_apiClient client;

        public geeksync_server_apiClient Client {get {return client;}}
        public System.Net.Http.HttpClient httpClient { get; }

        private bool isAvailable;

        public bool IsAvailable { get { return isAvailable; } }


        public Guid ChannelID{ get; }

        public GenericClient(Guid channelID, string baseUrl)
        {
            this.ChannelID = channelID;
            httpClient = new System.Net.Http.HttpClient();
            client = new geeksync_server_apiClient(baseUrl, httpClient);
        }

        public GenericClient(Guid channelID, string baseUrl, System.Net.Http.HttpClient httpClient)
        {
            this.ChannelID = channelID;
            this.httpClient = httpClient;
            client = new geeksync_server_apiClient(baseUrl, httpClient);
        }

        public bool CheckIfAvailable()
        {
            bool avail = false;

            try
            {
                client.ChannelAsync(this.ChannelID).RunSynchronously();
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