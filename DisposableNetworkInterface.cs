using System;
using System.Net;
using System.Net.Sockets;
using washared;

namespace qsrv
{
    public abstract class DisposableNetworkInterface : NetworkInterface, IDisposable
    {
        private protected readonly NetworkStream NetworkStream = null;
        private protected readonly Socket Socket = null;
#nullable enable

        private protected DisposableNetworkInterface(Socket? socket)
        {
            if (socket == null)
            {
                return;
            }
            Socket = socket;
            NetworkStream = new NetworkStream(socket);
        }

#nullable disable

        private protected DisposableNetworkInterface(string ip, int port)
        {
            bool success = IPAddress.TryParse(ip, out IPAddress ipAddress);
            if (!success)
            {
                throw new Exception("Unable to parse " + ip);
            }
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(ipEndPoint);
            NetworkStream = new NetworkStream(Socket);
        }

        public virtual void Dispose()
        {
            try
            {
                SslStream.Close();
                SslStream.Dispose();
            }
            catch (ObjectDisposedException) { }
            try
            {
                NetworkStream.Close();
                NetworkStream.Dispose();
            }
            catch (ObjectDisposedException) { }
            try
            {
                if (Socket.Connected)
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Disconnect(false);
                }
                Socket.Close();
            }
            catch (ObjectDisposedException) { }
        }
    }
}