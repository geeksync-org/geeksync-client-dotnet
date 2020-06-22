using System.Threading.Tasks;

using Xunit;
using System;
using System.Threading;
using GeekSyncClient.Client;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http;

//TODO test httpClient constructors too!
namespace GeekSyncClient.IntegrationTests
{
    public class ClientTests
        : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _fixture;

        public ClientTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CheckRandomChannelDefaultConfig()
        {
            new FileInfo(".geeksync-client.conf").Delete();
            ConfigManager config = new ConfigManager();

            SenderClient client = new SenderClient(config, config.Config.Peers[0].ChannelID, "http://localhost:5000/");
            client.CheckIfAvailable();

            Assert.False(client.IsAvailable);
            new FileInfo(".geeksync-client.conf").Delete();
        }

        [Fact]
        public void ChannelLifecycle()
        {
            new FileInfo(".test.2.conf").Delete();


            ConfigManager config = new ConfigManager(".test.2.conf");

            SenderClient sender = new SenderClient(config, config.Config.Peers[0].ChannelID, "http://localhost:5000/");
            ReceiverClient receiver = new ReceiverClient(config, config.Config.Peers[0].ChannelID, "http://localhost:5000/");

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            receiver.Connect();

            sender.CheckIfAvailable();
            Assert.True(sender.IsAvailable);

            receiver.Disconnect();

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            new FileInfo(".test.2.conf").Delete();

        }
        [Fact]
        public void ChannelLifecycleHttpClient()
        {
            new FileInfo(".test.2.conf").Delete();

            HttpClient httpClient=new HttpClient();
            httpClient.BaseAddress=new Uri("http://localhost:5000/");


            ConfigManager config = new ConfigManager(".test.2.conf");

            SenderClient sender = new SenderClient(config, config.Config.Peers[0].ChannelID,httpClient.BaseAddress.ToString(),httpClient);
            ReceiverClient receiver = new ReceiverClient(config, config.Config.Peers[0].ChannelID, httpClient.BaseAddress.ToString(),httpClient);

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            receiver.Connect();

            sender.CheckIfAvailable();
            Assert.True(sender.IsAvailable);

            receiver.Disconnect();

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            new FileInfo(".test.2.conf").Delete();

        }

        private string lastMsg = "Nothing received.";

        void receiverHanlder(string msg)
        {
            lastMsg = msg;
        }

        [Fact]
        public void SendAndReceive()
        {
            new FileInfo(".test.3r.conf").Delete();
            new FileInfo(".test.3s.conf").Delete();

            ConfigManager rconfig = new ConfigManager(".test.3r.conf");
            ConfigManager sconfig = new ConfigManager(".test.3s.conf");

            rconfig.PeerWith(sconfig);

            Guid ch = sconfig.Config.Peers.Single(x => x.PeerID == rconfig.Config.MyID).ChannelID;

            SenderClient sender = new SenderClient(sconfig, ch, "http://localhost:5000/");
            ReceiverClient receiver = new ReceiverClient(rconfig, ch, "http://localhost:5000/");

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            lastMsg = "";

            receiver.MessageReceived = receiverHanlder;

            receiver.Connect();



            sender.CheckIfAvailable();
            Assert.True(sender.IsAvailable);

            Assert.Equal("", lastMsg);
            sender.SendMessage("TEST MESSAGE");

            Thread.Sleep(10 * 1000);

            Assert.Equal("TEST MESSAGE", lastMsg);

            receiver.Disconnect();

            sender.CheckIfAvailable();
            Assert.False(sender.IsAvailable);

            new FileInfo(".test.3r.conf").Delete();
            new FileInfo(".test.3s.conf").Delete();

        }

    }

    public class ServerFixture : IDisposable
    {
        private Task task;
        private CancellationToken cancellationToken;
        private CancellationTokenSource cancellationTokenSource;
        public ServerFixture()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            task = Host.CreateDefaultBuilder(new string[] { })
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseStartup<GeekSyncServer.Startup>();
                            }).Build().RunAsync(cancellationToken);
        }

        public async void Dispose()
        {
            cancellationTokenSource.Cancel();
            await task;
            task.Dispose();
        }
    }
}