using System;
namespace GeekSyncClient.Dekstop
{
    public class DesktopAlreadyConnectedException:Exception
    {

        public DesktopAlreadyConnectedException(string message) 
            : base (message)
        {
        }

        public DesktopAlreadyConnectedException(string message,Exception inner) 
            : base (message,inner)
        {
        }
        public DesktopAlreadyConnectedException() 
            : base ("Desktop already connected.")
        {
        }

        public DesktopAlreadyConnectedException(Exception inner) 
            : base ("Desktop already connected.")
        {
        }
    }
}