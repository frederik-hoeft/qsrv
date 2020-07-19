using Newtonsoft.Json;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using qsrv.ApiRequests;
using qsrv.Database;
using washared;
using qsrv.ApiResponses;
using System.Threading;
using System;
using System.IO;
using System.Diagnostics;

namespace qsrv
{
    /// <summary>
    /// Main client handling looper thread
    /// </summary>
    public sealed class ApiServer : DisposableNetworkInterface
    {
        private bool isConnected = false;

        public bool SetupFailed { get; }

        public bool IsSynchonous { get; set; } = false;

        #region Constructor

        private ApiServer() : base(null)
        {
            Network = new Network(this);
            isConnected = true;
        }

        private ApiServer(Socket socket) : base(socket)
        {
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 10);
            Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.TcpKeepAliveRetryCount, 6);
            SslStream = new SslStream(NetworkStream);
            try
            {
                SslStream.AuthenticateAsServer(MainServer.ServerCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);
                SetupFailed = false;
            }
            catch (IOException e)
            {
                Console.WriteLine("Client disconnected due to " + e.Message);
                SetupFailed = true;
                Dispose();
                return;
            }
            Network = new Network(this);
            isConnected = true;
        }

#nullable enable

        public static void Create(Socket? socket)
        {
            if (socket == null)
            {
                MainServer.ClientCount--;
                return;
            }
            Console.WriteLine("Client connected from " + socket.RemoteEndPoint.ToString());
            ApiServer server = new ApiServer(socket);
            if (server.SetupFailed)
            {
                return;
            }
            server.Serve();
        }

        public void Send(string data)
        {
            if (!UnitTestDetector.IsInUnitTest && !MainServer.Config.WamsrvDevelopmentConfig.BlockResponses && isConnected)
            {
                Debug.WriteLine("(API) << " + data);
                try
                {
                    Network.Send(data);
                }
                catch (IOException)
                {
                    Network.Abort();
                }
            }
        }

        public static ApiServer CreateDummy()
        {
            return new ApiServer
            {
                IsSynchonous = true
            };
        }

#nullable disable

        #endregion Constructor

        private void Serve()
        {
            using PacketParser parser = new PacketParser(this)
            {
                PacketActionCallback = PacketActionCallback,
                UseMultiThreading = true,
                ReleaseResources = true,
                Interactive = false
            };
            try
            {
                parser.BeginParse();
            }
            catch (ConnectionDroppedException)
            {
                parser.Dispose();
                Dispose();
            }
        }

        private void PacketActionCallback(byte[] packet)
        {
            string json = Encoding.UTF8.GetString(packet);
            Debug.WriteLine("(API) >> " + json);
            try
            {
                SerializedApiRequest serializedApiRequest = JsonConvert.DeserializeObject<SerializedApiRequest>(json);
                ApiRequest apiRequest = serializedApiRequest.Deserialize();
                if (apiRequest == null)
                {
                    ApiError.Throw(ApiErrorCode.NotFound, new ApiContext(this, apiRequest.RequestId), "No request available under this ID.");
                    return;
                }
                apiRequest.Process(this);
            }
            catch (JsonException)
            {
                ApiError.Throw(ApiErrorCode.InternalServerError, new ApiContext(this, ApiRequestId.Invalid), "Failed to parse packet!");
            }
        }

        public override void Dispose()
        {
            isConnected = false;
            Finalizer();
            MainServer.ClientCount--;
            base.Dispose();
        }

        private void Finalizer()
        {
            // TODO: Write cached events to database
        }
    }
}