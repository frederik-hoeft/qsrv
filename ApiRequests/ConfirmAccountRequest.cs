using qsrv.ApiResponses;
using qsrv.Database;
using qsrv.Security;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.ApiRequests
{
    public class ConfirmAccountRequest : ApiRequest
    {
        public readonly string Code;

        public ConfirmAccountRequest(ApiRequestId requestId, string code)
        {
            RequestId = requestId;
            Code = code;
        }

        public async override void Process(ApiServer server)
        {
            if (server.AssertServerSetup(this) || server.AssertAuthenticationCodeInvalid(Code) || server.AssertUserOffline())
            {
                return;
            }
            using DatabaseManager databaseManager = new DatabaseManager(server);
            string userid = SecurityManager.GenerateHid();
            string query = DatabaseEssentials.Security.SanitizeQuery(new string[] { "INSERT INTO Tbl_user (password, hid, email) VALUES (\'", server.Account.Password, "\',\'", userid, "\', \'", server.Account.AccountInfo.Email, "\');" });
            SqlApiRequest sqlRequets = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optional = await databaseManager.GetModifyDataResponseAsync(sqlRequets);
            if (!optional.Success)
            {
                return;
            }
            SqlModifyDataResponse modifyDataResponse = optional.Result;
            if (!modifyDataResponse.Success)
            {
                ApiError.Throw(ApiErrorCode.InternalServerError, server, "Unable to create user.");
                return;
            }
            server.Account.AuthenticationCode = string.Empty;
            server.Account.AuthenticationId = ApiRequestId.Invalid;
            server.Account.AuthenticationTime = -1;
            GenericSuccessResponse response = new GenericSuccessResponse(ResponseId.ConfirmAccount, true);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            string json = serializedApiResponse.Serialize();
            server.Send(json);
            server.UnitTesting.MethodSuccess = true;
        }
    }
}