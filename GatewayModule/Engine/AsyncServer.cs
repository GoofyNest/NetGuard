using NetGuard.Services;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetGuard.Engine
{
    public sealed class AsyncServer : IDisposable
    {
        private Socket _listenerSocket = null;
        private E_ServerType _serverType;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _acceptTask;

        public enum E_ServerType : byte
        {
            GatewayModule
        }

        public delegate void DelClientDisconnect(ref Socket clientSocket, E_ServerType handlerType);

        public async Task StartAsync(string bindAddr, int port, E_ServerType servType)
        {
            if (_listenerSocket != null)
            {
                throw new Exception("Trying to start server on socket which is already in use");
            }

            _serverType = servType;
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Custom.WriteLine($"Redirect settings for {bindAddr}:{port} were loaded!");

                _listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(bindAddr), port));
                _listenerSocket.Listen(100);

                _acceptTask = AcceptConnectionsAsync(_cancellationTokenSource.Token);
                await _acceptTask;
            }
            catch (SocketException socketEx)
            {
                Custom.WriteLine($"Could not bind/listen/accept socket. Exception: {socketEx}", ConsoleColor.Red);
            }
        }

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var clientSocket = await _listenerSocket.AcceptAsync();
                    _ = Task.Run(() => HandleClientAsync(clientSocket), cancellationToken);
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Exception in AcceptConnectionsAsync: {ex.ToString()}", ConsoleColor.Red);
                }
            }
        }

        private async Task HandleClientAsync(Socket clientSocket)
        {
            try
            {
                switch (_serverType)
                {
                    case E_ServerType.GatewayModule:
                        new GatewayModule(clientSocket, OnClientDisconnect);
                        break;
                    default:
                        Custom.WriteLine("Unknown server type", ConsoleColor.Red);
                        break;
                }
            }
            catch (SocketException socketEx)
            {
                Custom.WriteLine($"Error while starting context. Exception: {socketEx}", ConsoleColor.Red);
            }
        }

        private void OnClientDisconnect(ref Socket clientSocket, E_ServerType handlerType)
        {
            if (clientSocket == null) return;

            try
            {
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Custom.WriteLine($"OnClientDisconnect()::Error closing socket. Exception: {ex}", ConsoleColor.Red);
            }

            clientSocket = null;
            GC.Collect();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _listenerSocket?.Close();
                _listenerSocket = null;
                _acceptTask?.Wait();
            }
        }

        ~AsyncServer()
        {
            Dispose(false);
        }
    }
}