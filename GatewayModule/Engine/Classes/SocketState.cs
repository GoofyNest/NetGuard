using System.Net.Sockets;

namespace NetGuard.Engine
{
    public class SocketState
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; }

        public SocketState(Socket socket, byte[] buffer)
        {
            Socket = socket;
            Buffer = buffer;
        }
    }
}
