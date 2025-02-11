using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Module.Classes;
using Module.Services;

namespace Module.Engine
{
    public sealed class AsyncServer : IDisposable
    {
        private Socket _listenerSocket = null!;
        private ModuleType _serverType;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _acceptTask = Task.CompletedTask;  // A completed task as a default value

        public delegate void DelClientDisconnect(ref Socket clientSocket, ModuleType handlerType);

        public async Task StartAsync(string bindAddr, int port, ModuleType servType)
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
                catch (SocketException) when (cancellationToken.IsCancellationRequested)
                {
                    Custom.WriteLine("Server shutting down gracefully.", ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Exception in AcceptConnectionsAsync: {ex}", ConsoleColor.Red);
                }
            }
        }

        private void HandleClientAsync(Socket clientSocket)
        {
            try
            {
                switch (_serverType)
                {
                    case ModuleType.GatewayModule:
                    case ModuleType.AgentModule:
                        new Module(clientSocket, OnClientDisconnect, _serverType);
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

        private void OnClientDisconnect(ref Socket clientSocket, ModuleType handlerType)
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

            clientSocket = null!;
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
                _listenerSocket = null!;
                _acceptTask?.Wait();
            }
        }

        ~AsyncServer()
        {
            Dispose(false);
        }
    }
}