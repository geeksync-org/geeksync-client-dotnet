using System;

namespace GeekSyncClient.Client
{
    public class SenderClient:GenericClient
    {

        public SenderClient(ConfigManager config,Guid channelID, string baseUrl)
            :base (config,channelID,baseUrl)
        {
        }

        public SenderClient(ConfigManager config,Guid channelID, string baseUrl, System.Net.Http.HttpClient httpClient)
            :base (config,channelID,baseUrl,httpClient)  
        {}     
         public void SendMessage(string message)
        {
            //TODO: better error handling
            if (CheckIfAvailable())
            {
                //string msg=.ToJSONString();
                Client.Channel4Async(this.ChannelID, MyRSA.EncryptAndSign(message));
            }
        }
    }
}