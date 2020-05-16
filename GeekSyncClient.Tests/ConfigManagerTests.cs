using System.Threading.Tasks;
using Xunit;
using System;
using System.Threading;
using GeekSyncClient;
using GeekSyncClient.Config;
using System.Xml.Linq;
using System.IO;
using System.Linq;

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

        [Fact]
        public void PairingTest()
        {
            string peer1conf=".peer.1.conf";
            string peer2conf=".peer.2.conf";

            FileInfo f1=new FileInfo(peer1conf);
            if (f1.Exists) f1.Delete();

            FileInfo f2=new FileInfo(peer2conf);
            if (f2.Exists) f2.Delete();
            
            ConfigManager peer1=new ConfigManager(peer1conf);
            ConfigManager peer2=new ConfigManager(peer2conf);


            Assert.NotEqual(peer1.Config.MyID,peer2.Config.MyID);
            Assert.Single(peer1.Config.Peers);
            Assert.Single(peer2.Config.Peers);

            Guid channelID=peer1.PeerWith(peer2);

            Assert.Single(peer1.Config.Peers.Where(x=>x.ChannelID==channelID));
            Assert.Single(peer2.Config.Peers.Where(x=>x.ChannelID==channelID));

            ConfigManager newPeer1=new ConfigManager(peer1conf);
            ConfigManager newPeer2=new ConfigManager(peer2conf);

            Assert.Single(newPeer1.Config.Peers.Where(x=>x.ChannelID==channelID));
            Assert.Single(newPeer2.Config.Peers.Where(x=>x.ChannelID==channelID));

            Peer p1=newPeer1.Config.Peers.Single(x=>x.ChannelID==channelID);
            Peer p2=newPeer2.Config.Peers.Single(x=>x.ChannelID==channelID);

            Assert.Equal(newPeer1.Config.MyID,p2.PeerID);
            Assert.Equal(newPeer1.Config.MyPublicKey,p2.PeerPublicKey);

            Assert.Equal(newPeer2.Config.MyID,p1.PeerID);
            Assert.Equal(newPeer2.Config.MyPublicKey,p1.PeerPublicKey);
        }




    }
}