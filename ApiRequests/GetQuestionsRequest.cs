using qsrv.ApiResponses;
using qsrv.Database;
using System;
using System.Collections.Generic;
using System.Text;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.ApiRequests
{
    public class GetQuestionsRequest : ApiRequest
    {
        public int Count { get; }
        public Category Category { get; } = Category.Undefined;

        public GetQuestionsRequest(int count, Category category)
        {
            Category = category;
            Count = count;
        }
        public async override void Process(ApiServer server)
        {
            server.UnitTesting.RequestId = RequestId;
            server.UnitTesting.MethodSuccess = false;
            if (Count < 1)
            {
                ApiError.Throw(ApiErrorCode.InvalidArgument, server, "Count cannot be less than 1.");
                server.UnitTesting.ErrorCode = ApiErrorCode.InvalidArgument;
                return;
            }
            using DatabaseManager databaseManager = new DatabaseManager(server);
            string query = "SELECT COUNT id FROM Tbl_questions" + (Category == Category.Undefined ? ";" : "WHERE category = " + ((int)Category).ToString() + ";");
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.GetSingleOrDefault, query, 1);
            Optional<SqlSingleOrDefaultResponse> optionalCount = await databaseManager.GetSingleOrDefaultResponseAsync(request);
            if (!optionalCount.Success || !optionalCount.Result.Success)
            {
                server.UnitTesting.ErrorCode = ApiErrorCode.DatabaseException;
                return;
            }
            int availableQuestions = Convert.ToInt32(optionalCount.Result.Result);
            int count = Math.Min(Count, availableQuestions);
            Question[] questions = new Question[count];
            query = "SELECT id, text, category FROM Tbl_questions WHERE id IN (SELECT id FROM Tbl_questions " + (Category == Category.Undefined ? string.Empty : "WHERE category = " + ((int)Category).ToString() + " ") + "ORDER BY RANDOM() LIMIT " + count.ToString() + ");";
            request = SqlApiRequest.Create(SqlRequestId.Get2DArray, query, 3);
            Optional<Sql2DArrayResponse> optionalQuestions = await databaseManager.Get2DArrayResponseAsync(request);
            if (!optionalQuestions.Success || !optionalQuestions.Result.Success)
            {
                server.UnitTesting.ErrorCode = ApiErrorCode.DatabaseException;
                return;
            }
            for (int i = 0; i < count; i++)
            {
                string[] question = optionalQuestions.Result.Result[i];
                questions[i] = new Question(question[0], question[1], (Category)Convert.ToInt32(question[2]));
                query = "SELECT text, isCorrect FROM Tbl_answers WHERE qid = " + question[0] + ";";
                request = SqlApiRequest.Create(SqlRequestId.Get2DArray, query, 2);
                Optional<Sql2DArrayResponse> optionalAnswers = await databaseManager.Get2DArrayResponseAsync(request);
                if (!optionalAnswers.Success || !optionalAnswers.Result.Success)
                {
                    server.UnitTesting.ErrorCode = ApiErrorCode.DatabaseException;
                    return;
                }
                if (optionalAnswers.Result.Result.Length != 4)
                {
                    ApiError.Throw(ApiErrorCode.InternalServerError, server, "Assertion error: Expected 4 answers for question #" + question[0] + " but got " + optionalAnswers.Result.Result.Length.ToString() + " instead!");
                    server.UnitTesting.ErrorCode = ApiErrorCode.InternalServerError;
                    return;
                }
                for (int j = 0; j < 4; j++)
                {
                    string[] answer = optionalAnswers.Result.Result[j];
                    questions[i].Answers[j] = new Answer(answer[0], Convert.ToBoolean(Convert.ToInt32(answer[1])));
                }
            }
            GetQuestionsResponse response = new GetQuestionsResponse(questions);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            string json = serializedApiResponse.Serialize();
            server.Network.Send(json);
            server.UnitTesting.MethodSuccess = true;
            server.UnitTesting.ErrorCode = ApiErrorCode.Ok;
        }
    }
}
