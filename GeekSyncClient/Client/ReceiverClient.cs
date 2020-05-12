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
        public WebsocketClient webSocketClient;

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


            var task = Task.Run(async () => { await Client.Channel2Async(ChannelID); });
            task.Wait();



            string ws_url = Client.BaseUrl.TrimEnd('/') + "/ws/" + ChannelID.ToString();

            //var exitEvent = new ManualResetEvent(false);

            webSocketClient = new WebsocketClient(new Uri(ws_url));


            webSocketClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
            webSocketClient.ReconnectionHappened.Subscribe(info =>


            webSocketClient.MessageReceived.Subscribe(msg => this.HandleMessage(msg)));
            webSocketClient.Start();

            //Task.Run(() => client.Send("{ message }"));

            //exitEvent.WaitOne();
        }

        public void Disconnect()
        {
            var task = Task.Run(async () => { await Client.Channel3Async(ChannelID); });
            task.Wait();

        }

        private void HandleMessage(ResponseMessage msg)
        {
            MessageReceived(msg.Text);
        }
    }
}
