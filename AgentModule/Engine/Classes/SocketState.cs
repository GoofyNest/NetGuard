using System.Net.Sockets;

namespace AgentModule.Engine.Classes
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
