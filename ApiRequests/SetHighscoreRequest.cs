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
        public SetHighscoreRequest(Highscore highscore) : base(ApiRequestId.SetHighscore)
        {
            Highscore = highscore;
        }
        private protected async override void ProcessAsync(ApiServer server, ManualResetEventSlim manualResetEvent)
        {
            using ProcessThreadHelper processThreadHelper = new ProcessThreadHelper(manualResetEvent);
            if (Highscore == null)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, Context, "Highscore cannot be null!");
                return;
            }
            if (Highscore.Score < 0)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, Context, "Highscore.Score cannot be less than 0!");
                return;
            }
            if (string.IsNullOrEmpty(Highscore.Player))
            {
                ApiError.Throw(ApiErrorCode.InvalidUser, Context, "Username cannot be null or empty.");
                return;
            }
            string player = InputSanitizer.Sanitize(Highscore.Player);
            string query = "INSERT INTO Tbl_highscore (player, score) VALUES (\'" + player + "\', " + Highscore.Score.ToString() + ");";
            using DatabaseManager databaseManager = new DatabaseManager(Context);
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optionalResponse = await databaseManager.GetModifyDataResponseAsync(request);
            if (!optionalResponse.Success || !optionalResponse.Result.Success)
            {
                return;
            }
            GenericSuccessResponse response = new GenericSuccessResponse(ResponseId.SetHighscore, true);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            string json = serializedApiResponse.Serialize();
            server.Send(json);
            Context.UnitTest.MethodSuccess = true;
            Context.UnitTest.ErrorCode = ApiErrorCode.Ok;
        }
    }
}
