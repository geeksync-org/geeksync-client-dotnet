using System.Security.Cryptography;
using System.Text;
using System;
using GeekSyncClient.Client;

namespace GeekSyncClient.Helper
{
    public class RSAHelper
    {
        public RSACryptoServiceProvider rsaMe;
        public RSACryptoServiceProvider rsaPeer;

        public string MyPublicKey {get {return Convert.ToBase64String(rsaMe.ExportCspBlob(false));}}

        public string KeysXml {get {return rsaMe.ToXmlString(true);}}

        public RSAHelper()
        {
            rsaMe = new RSACryptoServiceProvider(4096);
            rsaPeer= new RSACryptoServiceProvider(4096);
            //MyPublicKey= Convert.ToBase64String(rsaMe.ExportRSAPublicKey());
            rsaPeer.FromXmlString(rsaMe.ToXmlString(true));
        }

        public RSAHelper(string xmlKeyData)
        {
            rsaMe = new RSACryptoServiceProvider(4096);
            rsaPeer = new RSACryptoServiceProvider(4096);
            rsaMe.FromXmlString(xmlKeyData);
            //MyPublicKey= Convert.ToBase64String(rsaMe.ExportRSAPublicKey());
            rsaPeer.FromXmlString(rsaMe.ToXmlString(true));
        }

        public void SetPeerPublicKey(string peerPublicKey)
        { 
            rsaPeer.ImportCspBlob(Convert.FromBase64String(peerPublicKey));
        }


        public byte[] Sign(string message)
        {
            return Sign(Encoding.UTF8.GetBytes(message));
        }
        public byte[] Sign(byte[] data)
        {
            return rsaMe.SignData(data,HashAlgorithmName.SHA256,RSASignaturePadding.Pss);
        }

        public bool Verify(string message, byte[] signature)
        {
            return Verify(Encoding.UTF8.GetBytes(message),signature);

        }

        public bool Verify(byte[] data, byte[] signature)
        {
            return rsaPeer.VerifyData(data,signature,HashAlgorithmName.SHA256,RSASignaturePadding.Pss);

        }

        public byte[] Encrypt(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            return Encrypt(bytes);
        }

        public byte[] Encrypt(byte[] data)
        {
            return rsaPeer.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

        }

        public string Decrypt(byte[] encrypytedMessage)
        {
            byte[] bytes = rsaMe.Decrypt(encrypytedMessage, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(bytes);
        }


        public SignedMessage EncryptAndSign(string message)
        {
            SignedMessage signedMessage=new SignedMessage();
            byte[] encrypytedMessage=Encrypt(message);
            byte[] signature=Sign(encrypytedMessage);
            signedMessage.Message=Convert.ToBase64String(encrypytedMessage);
            signedMessage.Signature=Convert.ToBase64String(signature);
            return signedMessage;
        }

        public string VerifyAndDecrypt(SignedMessage signedMessage)
        {
            byte[] signature=Convert.FromBase64String(signedMessage.Signature);
            byte[] encrypytedMessage=Convert.FromBase64String(signedMessage.Message);

            if (Verify(encrypytedMessage,signature))
            {
                return Decrypt(encrypytedMessage);

            }
            else
            {
                return "Error: peer verification failed.";
            }
        }

     


    }
}