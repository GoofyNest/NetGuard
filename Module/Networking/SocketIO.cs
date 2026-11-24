using System.Net.Sockets;

namespace Module.Networking;

internal static class SocketIO
{
    internal static async ValueTask SendAllAsync(Socket socket, ReadOnlyMemory<byte> data,
        CancellationToken cancellationToken = default)
    {
        while (!data.IsEmpty)
        {
            int sent = await socket.SendAsync(data, SocketFlags.None, cancellationToken);
            if (sent == 0) throw new IOException("Socket closed during send.");
            data = data[sent..];
        }
    }
}
