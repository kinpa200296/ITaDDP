namespace UdpChat.Utility
{
    public interface IUdpChatCallback
    {
        void ProcessUdpMessage(byte[] data, int dataSize);
    }
}
