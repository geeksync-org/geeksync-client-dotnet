using System;
using System.Collections.Generic;


namespace GeekSyncClient.Config
{
    public class Peer
    {
        public Guid PeerID {get;set;}               
         public Guid ChannelID {get;set;}
        public string PeerPublicKey {get;set;}
    }
}