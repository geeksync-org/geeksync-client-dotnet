using System;
using Websocket.Client;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeekSyncClient.Client
{
    public class ReceiverClient : GenericClient
    {
        private WebsocketClient webSocket;

        public Action<string> MessageReceived;

        public ReceiverClient(Guid channelID, string baseUrl)
            : base(channelID, baseUrl)
        {
        }

        public ReceiverClient(Guid channelID, string baseUrl, System.Net.Http.HttpClient httpClient)
            : base(channelID, baseUrl, httpClient)
        { }

        public void Connect()
        {

            Client.Channel2Async(ChannelID).RunSynchronously();



            string ws_url = Client.BaseUrl.TrimEnd('/') + "/ws/" + ChannelID.ToString();

            //var exitEvent = new ManualResetEvent(false);

            webSocket = new WebsocketClient(new Uri(ws_url));


            webSocket.ReconnectTimeout = TimeSpan.FromSeconds(30);
            webSocket.ReconnectionHappened.Subscribe(info =>


            webSocket.MessageReceived.Subscribe(msg => this.HandleMessage(msg)));
            webSocket.Start();

            //Task.Run(() => client.Send("{ message }"));

            //exitEvent.WaitOne();
        }

        public void Disconnect()
        {

            Client.Channel3Async(ChannelID).RunSynchronously();
        }

        private void HandleMessage(ResponseMessage msg)
        {
            MessageReceived(msg.Text);
        }
    }
}
