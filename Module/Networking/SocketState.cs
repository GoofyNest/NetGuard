using System.Net.Sockets;

namespace Module.Networking
{
    public class SocketState(Socket socket, byte[] buffer)
    {
        public Socket Socket { get; set; } = socket;
        public byte[] Buffer { get; set; } = buffer;
    }
}
