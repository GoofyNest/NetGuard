using System.Net.Sockets;

namespace Module.Engine.Classes
{
    public class SocketState(Socket socket, byte[] buffer)
    {
        public Socket Socket { get; set; } = socket;
        public byte[] Buffer { get; set; } = buffer;
    }
}
