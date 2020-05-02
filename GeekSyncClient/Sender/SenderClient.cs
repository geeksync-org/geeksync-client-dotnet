using System;

namespace GeekSyncClient.Sender
{
    public class SenderClient
    {
        private sync_serviceClient client;
        public System.Net.Http.HttpClient httpClient { get; }

        private Guid pairingID { get; }

        private ChannelInfo _channelInfo;

        public ChannelInfo channelInfo { get { return _channelInfo; } }

        public bool IsChannelActive { get { return _channelInfo.Active; } }

        public System.Collections.Generic.ICollection<string> Desktops { get { return _channelInfo.Desktops; } }


        public SenderClient(Guid pairingID, string desktopName, string baseUrl)
        {
            this.pairingID = pairingID;
            httpClient = new System.Net.Http.HttpClient();
            client = new sync_serviceClient(baseUrl, httpClient);
            _channelInfo = RefreshChannelInfo();
        }

        public SenderClient(Guid pairingID, string desktopName, string baseUrl, System.Net.Http.HttpClient httpClient)
        {
            this.pairingID = pairingID;
            this.httpClient = httpClient;
            client = new sync_serviceClient(baseUrl, httpClient);
            _channelInfo = RefreshChannelInfo();
        }

        public ChannelInfo RefreshChannelInfo()
        {
            _channelInfo = client.ChannelAsync(this.pairingID).Result;
            return _channelInfo;
        }

        public void SendMessage(string message)
        {
            //TODO: better error handling
            MessagePayload payload=new MessagePayload();
            payload.PairingID=this.pairingID;
            payload.Message=message;
            client.Channel2Async(payload);
        }
    }
}