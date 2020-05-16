using System.Security.Cryptography;
using System.Text;
using System;

namespace GeekSyncClient.Helper
{
    public class RSAHelper
    {
        public RSA rsaMe;
        public RSA rsaPeer;

        public readonly string MyPublicKey;

        public RSAHelper()
        {
            rsaMe = RSA.Create();
            rsaPeer=RSA.Create();
            MyPublicKey= Convert.ToBase64String(rsaMe.ExportRSAPublicKey());
            rsaPeer.FromXmlString(rsaMe.ToXmlString(true));
        }

        public RSAHelper(string xmlKeyData)
        {
            rsaMe = RSA.Create();
            rsaPeer=RSA.Create();
            rsaMe.FromXmlString(xmlKeyData);
            MyPublicKey= Convert.ToBase64String(rsaMe.ExportRSAPublicKey());
            rsaPeer.FromXmlString(rsaMe.ToXmlString(true));
        }

        public void SetPeerPublicKey(string peerPublicKey)
        {
            int i = 0;
            rsaPeer.ImportRSAPublicKey(Convert.FromBase64String(peerPublicKey), out i);
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




    }
}