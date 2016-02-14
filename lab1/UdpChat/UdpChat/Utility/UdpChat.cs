using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UdpChat.Utility
{
    public class UdpChat
    {
        private int _port;
        private UdpSocket _udpSocket;
        private IPEndPoint _endPoint;
        private IUdpChatCallback _callback;
        private bool _done;
        private Task _listenerTask;
        private Dispatcher _dispatcher;

        public int Port
        {
            get { return _port; }
        }

        public bool Done
        {
            get { return _done; }
            private set { _done = value; }
        }

        public UdpChat(IUdpChatCallback callback, int port)
        {
            _callback = callback;
            _port = port;
            _setup();
        }

        public UdpChat(IUdpChatCallback callback)
        {
            _callback = callback;
            _port = 0xc0de;
            _setup();
        }

        private void _setup()
        {
            Done = true;
            _endPoint = new IPEndPoint(IPAddress.Any, Port);
            _udpSocket = new UdpSocket();
            _udpSocket.Bind(_endPoint);
        }

        public void Send(byte[] data)
        {
            _udpSocket.SendTo(data, new IPEndPoint(IPAddress.Broadcast, Port));
        }

        private void _StartListening()
        {
            Done = false;

            var buffer = new byte[_udpSocket.ReceiveBufferSize];

            while (!Done)
            {
                Thread.Sleep(10);
                try
                {
                    var size = _udpSocket.Receive(buffer);
                    _dispatcher.Invoke(() => _callback.ProcessUdpMessage(buffer, size));
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode != 10035)
                    {
                        throw;
                    }
                }
            }
        }

        public void StartListening()
        {
            if (Done)
            {
                _dispatcher = Dispatcher.CurrentDispatcher;
                _listenerTask = Task.Factory.StartNew(_StartListening);
            }
        }

        public void StopListening()
        {
            if (!Done)
            {
                Done = true;
                //_listenerTask.Wait();
            }
        }
    }
}
