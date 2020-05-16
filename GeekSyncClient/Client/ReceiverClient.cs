using System;
using System.Net.WebSockets;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.Encodings;

namespace GeekSyncClient.Client
{
    public class ReceiverClient : GenericClient
    {
        public ClientWebSocket webSocketClient;

        public Task ReceiveTask;

        public Action<string> MessageReceived;

        public ReceiverClient(ConfigManager config,Guid channelID, string baseUrl)
            : base(config,channelID, baseUrl)
        {
        }

        public ReceiverClient(ConfigManager config,Guid channelID, string baseUrl, System.Net.Http.HttpClient httpClient)
            : base(config,channelID, baseUrl, httpClient)
        { }

        public void Connect()
        {


            var task1 = Task.Run(async () => { await Client.Channel2Async(ChannelID); });
            task1.Wait();



            string ws_url = Client.BaseUrl.Replace("http:", "ws:").Replace("https:", "wss:").TrimEnd('/') + "/ws/" + ChannelID.ToString();

            var task2 = Task.Run(async () => { await openWebSocket(new Uri(ws_url)); });
            task2.Wait();

            ReceiveTask=Task.Run(async() =>{await Receive(webSocketClient);});
           
        }

        public async Task openWebSocket(Uri uri)
        {
            webSocketClient = new ClientWebSocket();

            //webSocketClient.Options.SetRequestHeader(headerName: "predix-zone-id", headerValue: PREDIX_ZONE_ID_HERE);
            //webSocketClient.Options.SetRequestHeader(headerName: "authorization", headerValue: "Bearer " + AUTH_TOKEN_HERE);
            //webSocketClient.Options.SetRequestHeader(headerName: "content-type", headerValue: "application/json");
            CancellationToken token = new CancellationToken();
            await webSocketClient.ConnectAsync(uri: uri, cancellationToken: token);

        }

        private async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms)) MessageReceived(MyRSA.VerifyAndDecrypt(SignedMessage.FromJSONString( await reader.ReadToEndAsync())));
                    
                }
            } while (true);
        }

        public void Disconnect()
        {
            //TODO: check how to close WebSocket nicely
            var task = Task.Run(async () => { await Client.Channel3Async(ChannelID); });
            task.Wait();

        }

        
    }
}
