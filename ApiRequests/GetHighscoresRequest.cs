using qsrv.ApiResponses;
using qsrv.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.ApiRequests
{
    public class GetHighscoresRequest : ApiRequest
    {
        public int Count { get; }
        public GetHighscoresRequest(int count)
        {
            Count = count;
        }

        private protected override async void ProcessAsync(ApiServer server, ManualResetEvent manualResetEvent)
        {
            using ProcessThreadHelper processThreadHelper = new ProcessThreadHelper(manualResetEvent);
            server.RequestId = RequestId;
            server.UnitTesting.RequestId = RequestId;
            server.UnitTesting.MethodSuccess = false;
            if (Count < 1)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, server, "Count cannot be less than 1.");
                server.UnitTesting.ErrorCode = ApiErrorCode.InvalidArgument;
                return;
            }
            using DatabaseManager databaseManager = new DatabaseManager(server);
            string query = "SELECT player, score FROM Tbl_highscore ORDER BY score DESC LIMIT " + Count.ToString() + ";";
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.Get2DArray, query, 2);
            Optional<Sql2DArrayResponse> optional2DArray = await databaseManager.Get2DArrayResponseAsync(request);
            if (!optional2DArray.Success || !optional2DArray.Result.Success)
            {
                server.UnitTesting.ErrorCode = ApiErrorCode.DatabaseException;
                return;
            }
            int count = optional2DArray.Result.Result.Length;
            Highscore[] highscores = new Highscore[count];
            for (int i = 0; i < count; i++)
            {
                string[] highscore = optional2DArray.Result.Result[i];
                highscores[i] = new Highscore(highscore[0], Convert.ToInt32(highscore[1]));
            }
            GetHighscoresResponse response = new GetHighscoresResponse(highscores);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            string json = serializedApiResponse.Serialize();
            server.Send(json);
            server.UnitTesting.MethodSuccess = true;
            server.UnitTesting.ErrorCode = ApiErrorCode.Ok;
        }
    }
}
