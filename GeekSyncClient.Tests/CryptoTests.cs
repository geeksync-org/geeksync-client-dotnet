using System.Threading.Tasks;
using Xunit;
using System;
using System.Threading;
using GeekSyncClient.Helper;
using System.Xml.Linq;

namespace GeekSyncClient.UnitTests
{
    public class CryptoTests

    {
        [Fact]
        public void CreateRSAHelperDefault()
        {
            RSAHelper helper = new RSAHelper();
            string xml = helper.rsaMe.ToXmlString(true);
            XDocument xd = XDocument.Parse(xml);
            XElement firstNode = (XElement)xd.FirstNode;
            Assert.Equal("RSAKeyValue", firstNode.Name);
            Assert.Equal(helper.rsaMe.ToXmlString(true),helper.rsaPeer.ToXmlString(true));
        }

        [Fact]
        public void CreateRSAHelperKeyFromXML()
        {
            RSAHelper helper1 = new RSAHelper();
            string xml1 = helper1.rsaMe.ToXmlString(true);
            XDocument xd1 = XDocument.Parse(xml1);
            XElement firstNode1 = (XElement)xd1.FirstNode;

            Assert.Equal("RSAKeyValue", firstNode1.Name);

            RSAHelper helper2 = new RSAHelper(xml1);
            string xml2 = helper2.rsaMe.ToXmlString(true);

            XDocument xd2 = XDocument.Parse(xml2);
            XElement firstNode2 = (XElement)xd2.FirstNode;

            Assert.Equal("RSAKeyValue", firstNode2.Name);

            Assert.Equal(xml1, xml2);

            Assert.Equal(helper2.rsaMe.ToXmlString(true),helper2.rsaPeer.ToXmlString(true));
            

        }

        [Fact]
        public void RSAHelperSelfEncryptAndDecrypt()
        {
            RSAHelper helper1 = new RSAHelper();
            string original = "This is test message";

            byte[] encrypted = helper1.Encrypt(original);
            string decrypted = helper1.Decrypt(encrypted);

            Assert.Equal(original, decrypted);

        }

        [Fact]
        public void RSAHelperCrossEncryptAndDecrypt()
        {
            RSAHelper receiver = new RSAHelper();
            RSAHelper sender = new RSAHelper();

            sender.SetPeerPublicKey(receiver.MyPublicKey);
            receiver.SetPeerPublicKey(sender.MyPublicKey);


            string original = "This is test message";

            byte[] encrypted = sender.Encrypt(original);
            string decrypted = receiver.Decrypt(encrypted);

            Assert.Equal(original, decrypted);

        }

        [Fact]
        public void RSAHelperSelfSignAndVerify()
        {
            RSAHelper helper1 = new RSAHelper();
            string original = "This is test message";
            byte[] signature = helper1.Sign(original);
            Assert.True(helper1.Verify(original,signature));

        }

        [Fact]
        public void RSAHelperCrossSignAndVerify()
        {
            RSAHelper receiver = new RSAHelper();
            RSAHelper sender = new RSAHelper();

            sender.SetPeerPublicKey(receiver.MyPublicKey);
            receiver.SetPeerPublicKey(sender.MyPublicKey);


            string original = "This is test message";

            byte[] signature = sender.Sign(original);
            Assert.True(receiver.Verify(original,signature));

        }





    }
}