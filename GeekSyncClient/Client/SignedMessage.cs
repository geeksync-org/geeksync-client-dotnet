using System;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace GeekSyncClient.Client
{
    public class SignedMessage
    {
        public string Message {get;set;}
        public string Signature {get;set;}

        public string ToJSONString()
        {
            return JsonSerializer.Serialize(this);
        }
        public static SignedMessage FromJSONString(string jsonString)
        {
            SignedMessage message=JsonSerializer.Deserialize<SignedMessage>(jsonString);
            return message;
        }
    }
}
