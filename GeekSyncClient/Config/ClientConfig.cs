using System;
using System.Collections.Generic;


namespace GeekSyncClient.Config
{
    public class ClientConfig
    {
        public Guid MyID {get;set;}
        public string RSAkeys{get;set;}

        public string MyPublicKey {get;set;}

        public List<Peer> Peers  {get;set;}
      
    }
}