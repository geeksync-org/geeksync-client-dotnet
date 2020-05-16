using System.Threading.Tasks;
using Xunit;
using System;
using System.Threading;
using GeekSyncClient;
using System.Xml.Linq;
using System.IO;

namespace GeekSyncClient.UnitTests
{
    public class PeerManagerTests

    {
        [Fact]
        public void InitialConfigTest()
        {
            string testConfigFileName=".test.client.1.conf";

            FileInfo f1=new FileInfo(testConfigFileName);
            if (f1.Exists) f1.Delete();

            ConfigManager peerManager=new ConfigManager(testConfigFileName);
            Assert.Single(peerManager.Config.Peers);

            FileInfo f2=new FileInfo(testConfigFileName);
            Assert.True(f2.Exists);
            f2.Delete();

        }

        [Fact]
        public void ConfigInitSaveLoad()
        {
            string testConfigFileName=".test.client2.conf";

            FileInfo f1=new FileInfo(testConfigFileName);
            if (f1.Exists) f1.Delete();

            ConfigManager peerManager=new ConfigManager(testConfigFileName);

            FileInfo f2=new FileInfo(testConfigFileName);
            Assert.True(f2.Exists);

            ConfigManager newPeerManager = new ConfigManager(testConfigFileName);

            Assert.Equal(peerManager.Config.MyID,newPeerManager.Config.MyID);
            Assert.Equal(peerManager.Config.RSAkeys,newPeerManager.Config.RSAkeys);
            Assert.Single(peerManager.Config.Peers);
            Assert.Single(newPeerManager.Config.Peers);
            Assert.Equal(peerManager.Config.Peers[0].PeerID,newPeerManager.Config.Peers[0].PeerID);

            f2.Delete();

        }






    }
}