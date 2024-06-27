using NetGuard.Services;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetGuard.Engine
{
    public sealed class AsyncServer
    {
        private Socket _listenerSocket;
        private E_ServerType _serverType;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public enum E_ServerType : byte
        {
            GatewayServer
        }

        public delegate void DelClientDisconnect(Socket clientSocket, E_ServerType handlerType);

        public async Task<bool> StartAsync(string bindAddr, int port, E_ServerType serverType)
        {
            if (_listenerSocket != null)
            {
                throw new InvalidOperationException("Server is already running.");
            }

            _serverType = serverType;
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Custom.WriteLine($"Redirect settings for {bindAddr}:{port} were loaded!");

                _listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(bindAddr), port));
                _listenerSocket.Listen(100); // Increased backlog for better performance

                await Task.Run(() => AcceptConnectionsAsync(_cancellationTokenSource.Token));
                return true;
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"Could not bind/listen socket. Exception: {ex}");
                return false;
            }
        }

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var clientSocket = await _listenerSocket.AcceptAsync();

                    _ = Task.Run(() => HandleConnectionAsync(clientSocket, cancellationToken), cancellationToken);
                }
                catch (SocketException ex)
                {
                    Custom.WriteLine($"AcceptConnectionCallback()::SocketException while EndAccept. Exception: {ex}");
                }
                catch (ObjectDisposedException ex)
                {
                    Custom.WriteLine($"AcceptConnectionCallback()::ObjectDisposedException while EndAccept. Is server shutting down? Exception: {ex}");
                }
            }
        }

        private async Task HandleConnectionAsync(Socket clientSocket, CancellationToken cancellationToken)
        {
            try
            {
                switch (_serverType)
                {
                    case E_ServerType.GatewayServer:
                        // Add context
                        new GatewayContext(clientSocket, OnClientDisconnect);
                        break;
                    default:
                        Custom.WriteLine($"AcceptConnectionCallback()::Unknown server type");
                        break;
                }
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"AcceptConnectionCallback()::Error while starting context. Exception: {ex}");
            }
        }

        private void OnClientDisconnect(Socket clientSocket, E_ServerType handlerType)
        {
            if (clientSocket == null) 
                return;

            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (SocketException ex)
            {
                Custom.WriteLine($"OnClientDisconnect()::Error closing socket. Exception: {ex}", ConsoleColor.Cyan);
            }
            catch (ObjectDisposedException ex)
            {
                Custom.WriteLine($"OnClientDisconnect()::Error closing socket (socket already disposed?). Exception: {ex}", ConsoleColor.Cyan);
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"Something went wrong with Async systems. Exception: {ex}", ConsoleColor.Cyan);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            if (_listenerSocket != null)
            {
                _listenerSocket.Close();
                _listenerSocket = null;
            }
        }
    }
}
