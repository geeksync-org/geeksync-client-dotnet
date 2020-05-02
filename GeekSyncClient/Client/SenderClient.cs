using System;

namespace GeekSyncClient.Client
{
    public class SenderClient:GenericClient
    {

        public SenderClient(Guid channelID, string baseUrl)
            :base (channelID,baseUrl)
        {
        }

        public SenderClient(Guid channelID, string baseUrl, System.Net.Http.HttpClient httpClient)
            :base (channelID,baseUrl,httpClient)  
        {}     
         public void SendMessage(string message)
        {
            //TODO: better error handling
            if (CheckIfAvailable())
            {
                Client.Channel4Async(this.ChannelID, message);
            }
        }
    }
}