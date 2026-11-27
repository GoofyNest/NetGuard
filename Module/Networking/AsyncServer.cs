using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Module.Config;
using Module.Services;

namespace Module.Networking
{
    public sealed class AsyncServer : IDisposable, IAsyncDisposable
    {
        private Socket _listenerSocket = null!;
        private ModuleType _serverType;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task _acceptTask = Task.CompletedTask;  // A completed task as a default value

        public delegate void DelClientDisconnect(int clientId, ref Socket clientSocket, ModuleType handlerType);

        private readonly ConcurrentDictionary<int, Module> _activeClients = new();
        private readonly ConcurrentDictionary<int, Task> _connectionTasks = new();
        private long _clientIdCounter = 0;
        private long _activeCount = 0;
        private int _disposed;
        private ConnectionAdmission _admission = null!;

        public async Task StartAsync(string bindAddr, int port, ModuleType servType)
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            if (_listenerSocket != null)
            {
                throw new Exception("Trying to start server on socket which is already in use");
            }

            _serverType = servType;
            Main.Module.ValidateLimits();
            _admission = new ConnectionAdmission(Main.Module.MaxConnections, Main.Module.MaxConnectionsPerIP);
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

        internal static TimeSpan AcceptRetryDelay(int failures)
            => TimeSpan.FromMilliseconds(100 * (1 << Math.Clamp(failures - 1, 0, 5)));

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            int failures = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Socket? accepted = null;
                try
                {
                    accepted = await _listenerSocket.AcceptAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    accepted.NoDelay = true;
                    var ip = ((IPEndPoint)accepted.RemoteEndPoint!).Address.ToString();
                    if (!_admission.TryAcquire(ip))
                    {
                        failures = 0;
                        continue;
                    }
                    int clientId = (int)Interlocked.Increment(ref _clientIdCounter) & int.MaxValue;
                    int released = 0;
                    Module module;
                    try
                    {
                        module = new Module(clientId, accepted,
                            (int id, ref Socket socket, ModuleType type) =>
                            {
                                if (Interlocked.Exchange(ref released, 1) == 0) _admission.Release(ip);
                                OnClientDisconnect(id, ref socket, type);
                            }, _serverType);
                    }
                    catch
                    {
                        if (Interlocked.Exchange(ref released, 1) == 0) _admission.Release(ip);
                        throw;
                    }
                    accepted = null; // Module now owns the accepted socket.
                    if (!_activeClients.TryAdd(clientId, module)) { module.Stop(); continue; }
                    Interlocked.Increment(ref _activeCount);
                    var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                    _connectionTasks[clientId] = completion.Task;
                    _ = RunClientLifetimeAsync(module, completion, cancellationToken);
                    failures = 0;
                }
                catch (Exception ex) when (cancellationToken.IsCancellationRequested
                    && ex is SocketException or OperationCanceledException or ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    Custom.WriteLine($"Exception in AcceptConnectionsAsync: {ex}", ConsoleColor.Red);
                    // Close an unowned socket before waiting, even if option setup failed.
                    accepted?.Dispose();
                    accepted = null;
                    failures = Math.Min(failures + 1, 6);
                    try { await Task.Delay(AcceptRetryDelay(failures), cancellationToken); }
                    catch (OperationCanceledException) { break; }
                }
                finally { accepted?.Dispose(); }
            }
        }

        private async Task RunClientLifetimeAsync(Module module, TaskCompletionSource completion, CancellationToken token)
        {
            try
            {
                await HandleClientAsync(module, token);
                await module.WaitForShutdownAsync();
            }
            catch (Exception ex)
            {
                module.Stop();
                Custom.WriteLine($"Client {module.ClientId} shutdown failed: {ex}", ConsoleColor.Red);
            }
            finally
            {
                completion.TrySetResult();
                _connectionTasks.TryRemove(module.ClientId, out _);
            }
        }

        private async Task HandleClientAsync(Module module, CancellationToken cancellationToken)
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
                            await module.StartAsync(cancellationToken);
                        }
                        break;
                    default:
                        Custom.WriteLine("Unknown server type", ConsoleColor.Red);
                        module.Stop();
                        break;
                }
            }
            catch (SocketException socketEx)
            {
                Custom.WriteLine($"Error while starting context. Exception: {socketEx}", ConsoleColor.Red);
                module.Stop();
            }
            catch (Exception ex)
            {
                module.Stop();
                if (!cancellationToken.IsCancellationRequested)
                    Custom.WriteLine($"Client startup failed: {ex}", ConsoleColor.Red);
            }
        }

        public int GetActiveConnections() => (int)Interlocked.Read(ref _activeCount);

        private void OnClientDisconnect(int clientId, ref Socket clientSocket, ModuleType handlerType)
        {
            // Ensure the client is removed here
            try
            {
                if (_activeClients.TryRemove(clientId, out _))
                {
                    Interlocked.Decrement(ref _activeCount);
                    Custom.WriteLine($"Client {clientId} disconnected. Total connections: {GetActiveConnections()}", ConsoleColor.Yellow);
                }
            }
            catch(Exception ex) 
            {
                Custom.WriteLine($"Client {clientId} failed to remove from list", ConsoleColor.Yellow);
                Custom.WriteLine($"{ex}");
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
                    clientSocket?.Dispose();
                    clientSocket = null!;
                }
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
            _cancellationTokenSource.Cancel();
            _listenerSocket?.Dispose();
            foreach (var module in _activeClients.Values) module.Stop();
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();
            try { await _acceptTask; }
            catch (OperationCanceledException) { }
            // Acceptance has stopped; no new lifetime tasks can now be added.
            foreach (var module in _activeClients.Values) module.Stop();
            await Task.WhenAll(_connectionTasks.Values);
        }
    }
}
