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
        Socket m_ListenerSock = null;
        E_ServerType m_ServerType;

        ManualResetEvent m_Waiter = new ManualResetEvent(false);
        Thread m_AcceptInitThread = null;

        public enum E_ServerType : byte
        {
            GatewayServer
        }

        public delegate void delClientDisconnect(ref Socket ClientSocket, E_ServerType HandlerType);

        public bool Start(string BindAddr, int nPort, E_ServerType ServType)
        {
            bool res = false;
            if (m_ListenerSock != null)
            {
                throw new Exception("Trying to start server on socket which is already in use");
            }

            m_ServerType = ServType;
            m_ListenerSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Custom.WriteLine("Redirect settings for {" + BindAddr + ":" + nPort + "} was loaded!");

                m_ListenerSock.Bind(new IPEndPoint(IPAddress.Parse(BindAddr), nPort));
                m_ListenerSock.Listen(100);

                m_AcceptInitThread = new Thread(AcceptInitThread);
                m_AcceptInitThread.Start();
            }
            catch (SocketException SocketEx)
            {
                Custom.WriteLine($"Could not bind/listen/BeginAccept socket. Exception: {SocketEx.ToString()}", ConsoleColor.Red);
            }

            return res;
        }

        void AcceptInitThread()
        {
            while (m_ListenerSock != null)
            {
                m_Waiter.Reset();
                try
                {
                    m_ListenerSock.BeginAccept(
                        new AsyncCallback(AcceptConnectionCallback), null
                        );
                }
                catch { }
                m_Waiter.WaitOne();
            }
        }

        void AcceptConnectionCallback(IAsyncResult iar)
        {
            Socket ClientSocket = null;

            //AcceptInitThread sleeps...
            m_Waiter.Set();

            try
            {
                ClientSocket = m_ListenerSock.EndAccept(iar);
            }

            catch (SocketException SocketEx)
            {
                Custom.WriteLine($"AcceptConnectionCallback()::SocketException while EndAccept. Exception: {SocketEx.ToString()}", ConsoleColor.Red);

            }
            catch (ObjectDisposedException ObjDispEx)
            {
                Custom.WriteLine($"AcceptConnectionCallback()::ObjectDisposedException while EndAccept. Is server shutting down ? Exception: {ObjDispEx.ToString()}", ConsoleColor.Red);
            }

            try
            {
                switch (m_ServerType)
                {
                    case E_ServerType.GatewayServer:
                        {
                            // Add context
                            new GatewayContext(ClientSocket, OnClientDisconnect);
                        }
                        break;
                    default:
                        {
                            Custom.WriteLine("AcceptConnectionCallback()::Unknown server type", ConsoleColor.Red);
                        }
                        break;
                }
            }
            catch (SocketException SocketEx)
            {
                Custom.WriteLine($"AcceptConnectionCallback()::Error while starting context. Exception: {SocketEx.ToString()}", ConsoleColor.Red);
            }
        }

        void OnClientDisconnect(ref Socket ClientSock, E_ServerType HandlerType)
        {
            // Check
            if (ClientSock == null)
            {
                return;
            }

            try
            {
                ClientSock.Close();
            }
            catch (SocketException SocketEx)
            {
                Custom.WriteLine($"OnClientDisconnect()::Error closing socket. Exception: {SocketEx.ToString()}", ConsoleColor.Red);
            }
            catch (ObjectDisposedException ObjDispEx)
            {
                Custom.WriteLine($"OnClientDisconnect()::Error closing socket (socket already disposed?). Exception: {ObjDispEx.ToString()}", ConsoleColor.Red);
            }
            catch
            {
                Custom.WriteLine("Something went wrong with Async systems.", ConsoleColor.Red);
            }


            ClientSock = null;
            GC.Collect();
        }

        //public async Task<bool> StartAsync(string bindAddr, int port, E_ServerType serverType)
        //{
        //    if (_listenerSocket != null)
        //    {
        //        throw new InvalidOperationException("Server is already running.");
        //    }
        //
        //    _serverType = serverType;
        //    _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //
        //    try
        //    {
        //        Custom.WriteLine($"Redirect settings for {bindAddr}:{port} were loaded!");
        //
        //        _listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(bindAddr), port));
        //        _listenerSocket.Listen(100); // Increased backlog for better performance
        //
        //        await Task.Run(() => AcceptConnectionsAsync(_cancellationTokenSource.Token));
        //        return true;
        //    }
        //    catch (SocketException ex)
        //    {
        //        Custom.WriteLine($"Could not bind/listen socket. Exception: {ex}");
        //        return false;
        //    }
        //}
        //
        //private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        //{
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            var clientSocket = await _listenerSocket.AcceptAsync();
        //
        //            _ = Task.Run(() => HandleConnectionAsync(clientSocket, cancellationToken), cancellationToken);
        //        }
        //        catch (SocketException ex)
        //        {
        //            Custom.WriteLine($"AcceptConnectionCallback()::SocketException while EndAccept. Exception: {ex}");
        //        }
        //        catch (ObjectDisposedException ex)
        //        {
        //            Custom.WriteLine($"AcceptConnectionCallback()::ObjectDisposedException while EndAccept. Is server shutting down? Exception: {ex}");
        //        }
        //    }
        //}
        //
        //private async Task HandleConnectionAsync(Socket clientSocket, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        switch (_serverType)
        //        {
        //            case E_ServerType.GatewayServer:
        //                // Add context
        //                new GatewayContext(clientSocket, OnClientDisconnect);
        //                break;
        //            default:
        //                Custom.WriteLine($"AcceptConnectionCallback()::Unknown server type");
        //                break;
        //        }
        //    }
        //    catch (SocketException ex)
        //    {
        //        Custom.WriteLine($"AcceptConnectionCallback()::Error while starting context. Exception: {ex}");
        //    }
        //}
        //
        //private void OnClientDisconnect(Socket clientSocket, E_ServerType handlerType)
        //{
        //    if (clientSocket == null) 
        //        return;
        //
        //    try
        //    {
        //        clientSocket.Close();
        //    }
        //    catch (SocketException ex)
        //    {
        //        Custom.WriteLine($"OnClientDisconnect()::Error closing socket. Exception: {ex}", ConsoleColor.Cyan);
        //    }
        //    catch (ObjectDisposedException ex)
        //    {
        //        Custom.WriteLine($"OnClientDisconnect()::Error closing socket (socket already disposed?). Exception: {ex}", ConsoleColor.Cyan);
        //    }
        //    catch (Exception ex)
        //    {
        //        Custom.WriteLine($"Something went wrong with Async systems. Exception: {ex}", ConsoleColor.Cyan);
        //    }
        //
        //    clientSocket = null;
        //}
        //
        //public void Stop()
        //{
        //    _cancellationTokenSource.Cancel();
        //
        //    if (_listenerSocket != null)
        //    {
        //        _listenerSocket.Close();
        //        _listenerSocket = null;
        //    }
        //}
    }
}
