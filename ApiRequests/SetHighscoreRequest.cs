using qsrv.ApiResponses;
using qsrv.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.ApiRequests
{
    public class SetHighscoreRequest : ApiRequest
    {
        public Highscore Highscore { get; }
        public SetHighscoreRequest(Highscore highscore)
        {
            Highscore = highscore;
        }
        private protected async override void ProcessAsync(ApiServer server, ManualResetEvent manualResetEvent)
        {
            using ProcessThreadHelper processThreadHelper = new ProcessThreadHelper(manualResetEvent);
            server.RequestId = RequestId;
            server.UnitTesting.MethodSuccess = false;
            server.UnitTesting.RequestId = RequestId;
            if (Highscore == null)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, server, "Highscore cannot be null!");
                server.UnitTesting.ErrorCode = ApiErrorCode.InvalidArgument;
                return;
            }
            if (Highscore.Score < 0)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, server, "Highscore.Score cannot be less than 0!");
                server.UnitTesting.ErrorCode = ApiErrorCode.InvalidArgument;
                return;
            }
            if (string.IsNullOrEmpty(Highscore.Player))
            {
                ApiError.Throw(ApiErrorCode.InvalidUser, server, "Username cannot be null or empty.");
                server.UnitTesting.ErrorCode = ApiErrorCode.InvalidUser;
                return;
            }
            string player = InputSanitizer.Sanitize(Highscore.Player);
            string query = "INSERT INTO Tbl_highscore (player, score) VALUES (\'" + player + "\', " + Highscore.Score.ToString() + ");";
            using DatabaseManager databaseManager = new DatabaseManager(server);
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optionalResponse = await databaseManager.GetModifyDataResponseAsync(request);
            if (!optionalResponse.Success || !optionalResponse.Result.Success)
            {
                server.UnitTesting.ErrorCode = ApiErrorCode.DatabaseException;
                return;
            }
            GenericSuccessResponse response = new GenericSuccessResponse(ResponseId.SetHighscore, true);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            string json = serializedApiResponse.Serialize();
            server.Send(json);
            server.UnitTesting.MethodSuccess = true;
            server.UnitTesting.ErrorCode = ApiErrorCode.Ok;
        }
    }
}
