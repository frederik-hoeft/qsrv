using Newtonsoft.Json;
using System;
using System.Diagnostics;
using qsrv.ApiResponses;
using washared;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;
using ApiResponse = washared.DatabaseServer.ApiResponses.ApiResponse;
using System.Threading.Tasks;

namespace qsrv.Database
{
    /// <summary>
    /// To be used with 'using -> opens connection to local wadbsrv.exe
    /// </summary
    public partial class DatabaseManager : IDisposable
    {
        private SqlClient sqlClient = null;
        private bool isInitialized = false;
        private PacketParser packetParser = null;
        private readonly ApiServer server;

        public DatabaseManager(ApiServer server)
        {
            this.server = server;
        }

        private void Inititalize()
        {
            if (isInitialized)
            {
                return;
            }
            sqlClient = new SqlClient(MainServer.Config.WamsrvInterfaceConfig.DatabaseServerIp, MainServer.Config.WamsrvInterfaceConfig.DatabaseServerPort);
            packetParser = new PacketParser(sqlClient)
            {
                Interactive = true,
                PacketActionCallback = null,
                ReleaseResources = true,
                UseBackgroundParsing = true,
            };
            packetParser.BeginParse();
            isInitialized = true;
        }

        private async Task<DatabaseResult> GetResponseAsync(SqlApiRequest request)
        {
            Inititalize();
            string jsonRequest = request.Serialize();
            sqlClient.Network.Send(jsonRequest);
            byte[] packet = await packetParser.GetPacket(10000);
            ApiResponse response;
            if (packet.Length == 0)
            {
                response = null;
            }
            else
            {
                string jsonResponse = sqlClient.Network.Encoding.GetString(packet);
                Debug.WriteLine(">> " + jsonResponse);
                SerializedSqlApiResponse serializedApiResponse = JsonConvert.DeserializeObject<SerializedSqlApiResponse>(jsonResponse);
                response = serializedApiResponse.Deserialize();
            }
            if (response == null)
            {
                ApiError.Throw(ApiErrorCode.InternalServerError, server, "Database request timed out.");
                return new DatabaseResult(null, false);
            }
            if (response.ResponseId == SqlResponseId.Error)
            {
                SqlErrorResponse sqlError = (SqlErrorResponse)response;
                ApiError.Throw(ApiErrorCode.DatabaseException, server, sqlError.Message);
                return new DatabaseResult(null, false);
            }
            return new DatabaseResult(response, true);
        }

        public async Task<Optional<SqlModifyDataResponse>> GetModifyDataResponseAsync(SqlApiRequest request)
        {
            DatabaseResult databaseResult = await GetResponseAsync(request);
            return new Optional<SqlModifyDataResponse>((SqlModifyDataResponse)databaseResult.ApiResponse, databaseResult.Success);
        }

        public async Task<Optional<Sql2DArrayResponse>> Get2DArrayResponseAsync(SqlApiRequest request)
        {
            DatabaseResult databaseResult = await GetResponseAsync(request);
            return new Optional<Sql2DArrayResponse>((Sql2DArrayResponse)databaseResult.ApiResponse, databaseResult.Success);
        }

        public async Task<Optional<SqlDataArrayResponse>> GetDataArrayResponseAsync(SqlApiRequest request)
        {
            DatabaseResult databaseResult = await GetResponseAsync(request);
            return new Optional<SqlDataArrayResponse>((SqlDataArrayResponse)databaseResult.ApiResponse, databaseResult.Success);
        }

        public async Task<Optional<SqlSingleOrDefaultResponse>> GetSingleOrDefaultResponseAsync(SqlApiRequest request)
        {
            DatabaseResult databaseResult = await GetResponseAsync(request);
            return new Optional<SqlSingleOrDefaultResponse>((SqlSingleOrDefaultResponse)databaseResult.ApiResponse, databaseResult.Success);
        }

        public void Dispose()
        {
            if (!isInitialized)
            {
                return;
            }
            packetParser.ShutdownAsync();
            if (sqlClient != null)
            {
                try
                {
                    sqlClient.Dispose();
                }
                catch (ObjectDisposedException) { }
            }
        }
    }
}