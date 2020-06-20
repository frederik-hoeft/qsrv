using Newtonsoft.Json;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using qsrv.ApiRequests;
using qsrv.Database;
using washared;

namespace qsrv
{
    /// <summary>
    /// Main client handling looper thread
    /// </summary>
    public sealed class ApiServer : DisposableNetworkInterface
    {
        public Account Account { get; set; } = null;
        public readonly UnitTesting UnitTesting = new UnitTesting();
        private ApiRequestId requestId = ApiRequestId.Invalid;
        private bool isConnected = false;

        public ApiRequestId RequestId
        {
            get { return requestId; }
            set
            {
                requestId = value;
                UnitTesting.RequestId = value;
            }
        }

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
            Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 6);
            SslStream = new SslStream(NetworkStream);
            SslStream.AuthenticateAsServer(MainServer.ServerCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);
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
            ApiServer server = new ApiServer(socket);
            server.Serve();
        }

        public void Send(string data)
        {
            if (!UnitTestDetector.IsInUnitTest && !MainServer.Config.WamsrvDevelopmentConfig.BlockResponses && isConnected)
            {
                Network.Send(data);
            }
        }

        public static ApiServer CreateDummy()
        {
            return new ApiServer();
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
            SerializedApiRequest serializedApiRequest = JsonConvert.DeserializeObject<SerializedApiRequest>(json);
            ApiRequest apiRequest = serializedApiRequest.Deserialize();
            apiRequest.Process(this);
        }

        public async override void Dispose()
        {
            isConnected = false;
            Finalizer();
            try
            {
                if (Account != null && !string.IsNullOrEmpty(Account.Id))
                {
                    using DatabaseManager databaseManager = new DatabaseManager(this);
                    await databaseManager.SetUserOffline();
                }
            }
            catch { }
            MainServer.ClientCount--;
            base.Dispose();
        }

        private void Finalizer()
        {
            // TODO: Write cached events to database
        }
    }
}