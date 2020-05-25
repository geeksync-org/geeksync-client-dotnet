using System;
using Newtonsoft.Json;


namespace GeekSyncClient.Client
{
    public class SignedMessage
    {
        public string Message {get;set;}
        public string Signature {get;set;}

        public string ToJSONString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static SignedMessage FromJSONString(string jsonString)
        {
            SignedMessage message=JsonConvert.DeserializeObject<SignedMessage>(jsonString);
            return message;
        }
    }
}
