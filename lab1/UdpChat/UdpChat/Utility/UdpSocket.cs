using System.Net.Sockets;

namespace UdpChat.Utility
{
    public class UdpSocket : Socket
    {
        #region constructors

        public UdpSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        {
            _setup();
        }

        public UdpSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
            _setup();
        }

        public UdpSocket(SocketInformation socketInformation) : base(socketInformation)
        {
            _setup();
        }

        public UdpSocket() : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            _setup();
        }

        #endregion

        private void _setup()
        {
            Blocking = false;
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        }
    }
}
