using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Module.Classes;
using Module.Services;
using static Module.Helpers.PacketManager.Gateway.Opcodes;

namespace Module.Engine
{
    public sealed class AsyncServer : IDisposable
    {
        private Socket _listenerSocket = null!;
        private ModuleType _serverType;
        private CancellationTokenSource _cancellationTokenSource = new();
        private Task _acceptTask = Task.CompletedTask;  // A completed task as a default value

        public delegate void DelClientDisconnect(int clientId, ref Socket clientSocket, ModuleType handlerType);

        private readonly ConcurrentDictionary<int, Module> _activeClients = new();
        private long _clientIdCounter = 0;

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
                _listenerSocket.Listen(2500);

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

                    // If Module constructor does async work, ensure it’s handled asynchronously
                    int clientId = (int)Interlocked.Increment(ref _clientIdCounter) & int.MaxValue;
                    var module = new Module(clientId, clientSocket, OnClientDisconnect, _serverType);

                    if (_activeClients.TryAdd(clientId, module))
                    {
                        Custom.WriteLine($"Client {clientId} connected. Total connections: {GetActiveConnections()}", ConsoleColor.Cyan);
                    }

                    // Use the cancellation token directly here
                    _ = Task.Run(() => HandleClientAsync(clientId, module, cancellationToken), cancellationToken);
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

        private async Task HandleClientAsync(int clientId, Module module, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                switch (_serverType)
                {
                    case ModuleType.GatewayModule:
                    case ModuleType.AgentModule:
                        {
                            // If the Module class uses async operations, you might need to await it
                            await module.StartAsync(cancellationToken);  // Assuming there's an async method to handle client in Module
                        }
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

        public int GetActiveConnections()
        {
            return _activeClients.Count;
        }

        private void OnClientDisconnect(int clientId, ref Socket clientSocket, ModuleType handlerType)
        {
            // Ensure the client is removed here
            try
            {
                if (_activeClients.TryRemove(clientId, out _))
                {
                    Custom.WriteLine($"Client {clientId} disconnected. Total connections: {GetActiveConnections()}", ConsoleColor.Yellow);
                }
            }
            catch(Exception ex) 
            {
                Custom.WriteLine($"Client {clientId} failed to remove from list", ConsoleColor.Yellow);
                Custom.WriteLine($"{ex.ToString()}");
            }

            // Handle socket cleanup
            if (clientSocket != null)
            {
                try
                {
                    if (clientSocket.Connected)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                    }
                    clientSocket.Close();
                }
                catch (Exception ex)
                {
                    Custom.WriteLine($"OnClientDisconnect()::Error closing socket. Exception: {ex}", ConsoleColor.Red);
                }
                finally
                {
                    clientSocket = null!;
                }
            }
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
                _listenerSocket?.Dispose();  // ✅ Use Dispose() instead of Close()
                _listenerSocket = null!;
            }
        }

        public async ValueTask DisposeAsync()
        {
            Dispose(true);
            if (_acceptTask != null)
            {
                await _acceptTask;  // ✅ Prevents deadlocks
            }
            GC.SuppressFinalize(this);
        }

        ~AsyncServer()
        {
            Dispose(false);
        }
    }
}