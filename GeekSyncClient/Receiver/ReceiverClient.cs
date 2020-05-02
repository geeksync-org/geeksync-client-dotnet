using System;
using GeekSyncClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Websocket.Client;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeekSyncClient.Receiver
{
    public class ReceiverClient
    {
        private sync_serviceClient client;
        public System.Net.Http.HttpClient httpClient { get; }

        private Guid desktopID { get; }
        private string desktopName { get; }

        public Action<string> MessageReceived;

        private Dictionary<Guid, ChannelInfo> pairings = new Dictionary<Guid, ChannelInfo>();
        private Dictionary<Guid,WebsocketClient> websockets =new Dictionary<Guid, WebsocketClient>();

        public ReceiverClient(Guid desktopID, string desktopName, string baseUrl)
        {
            this.desktopID = desktopID;
            this.desktopName = desktopName;
            httpClient = new System.Net.Http.HttpClient();
            client = new sync_serviceClient(baseUrl, httpClient);
        }

        public ReceiverClient(Guid desktopID, string desktopName, string baseUrl, System.Net.Http.HttpClient httpClient)
        {
            this.desktopID = desktopID;
            this.desktopName = desktopName;
            this.httpClient = httpClient;
            client = new sync_serviceClient(baseUrl, httpClient);
        }

        public void Disconnect(Guid pairingID)
        {

            client.DisconnectAsync(pairingID, desktopID);
            websockets[pairingID].Stop(WebSocketCloseStatus.NormalClosure,"Bye!");
            pairings.Remove(pairingID);

        }

        public void DisconnectAll()
        {
            foreach (Guid p in pairings.Keys.ToArray())
            {
                pairings.Remove(p);
            }
        }

        public void ReconnectAll()
        {
            foreach (Guid p in pairings.Keys.ToArray())
            {
                this.Connect(p, true);
            }
        }

        public void Connect(Guid pairingID, bool force = false)
        {
            if (pairings.ContainsKey(pairingID))
            {
                if (force)
                {
                    try
                    {
                        this.Disconnect(pairingID);
                    }
                    catch
                    {
                        pairings.Remove(pairingID);
                        websockets.Remove(pairingID);
                    }
                    
                }
                else
                {
                    throw new DesktopAlreadyConnectedException();
                }
            }
            ChannelInfo info = client.ConnectAsync(pairingID, desktopID, desktopName).Result;
            pairings.Add(pairingID, info);


            string ws_url = client.BaseUrl.TrimEnd('/') + "/ws/" + pairingID.ToString() + "/" + desktopID.ToString();

            //var exitEvent = new ManualResetEvent(false);
           
            using (var ws_client = new WebsocketClient(new Uri(ws_url)))
            {
                websockets.Add(pairingID,ws_client);
                ws_client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                ws_client.ReconnectionHappened.Subscribe(info =>


                ws_client.MessageReceived.Subscribe(msg => this.HandleMessage(msg)));
                ws_client.Start();

                //Task.Run(() => client.Send("{ message }"));

                //exitEvent.WaitOne();
            }

        }

        private void HandleMessage(ResponseMessage msg)
        {
           MessageReceived(msg.Text);
        }
    }

}