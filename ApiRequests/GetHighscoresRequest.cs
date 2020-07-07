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
        public GetHighscoresRequest(int count) : base(ApiRequestId.GetHighscores)
        {
            Count = count;
        }

        private protected override async void ProcessAsync(ApiServer server, ManualResetEventSlim manualResetEvent)
        {
            using ProcessThreadHelper processThreadHelper = new ProcessThreadHelper(manualResetEvent);
            if (Count < 1)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, Context, "Count cannot be less than 1.");
                return;
            }
            using DatabaseManager databaseManager = new DatabaseManager(Context);
            string query = "SELECT player, score FROM Tbl_highscore ORDER BY score DESC LIMIT " + Count.ToString() + ";";
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.Get2DArray, query, 2);
            Optional<Sql2DArrayResponse> optional2DArray = await databaseManager.Get2DArrayResponseAsync(request);
            if (!optional2DArray.Success || !optional2DArray.Result.Success)
            {
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
            Context.UnitTest.MethodSuccess = true;
            Context.UnitTest.ErrorCode = ApiErrorCode.Ok;
        }
    }
}
