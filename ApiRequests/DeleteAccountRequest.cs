using qsrv.ApiResponses;
using qsrv.Database;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.ApiRequests
{
    public class DeleteAccountRequest : ApiRequest
    {
        public DeleteAccountRequest(ApiRequestId requestId)
        {
            RequestId = requestId;
        }

        public async override void Process(ApiServer server)
        {
            if (server.AssertServerSetup(this) || server.AssertUserOnline() || server.AssertIdSet())
            {
                return;
            }
            using DatabaseManager databaseManager = new DatabaseManager(server);
            if (await databaseManager.OptionalAssertUserExists(server.Account.Id, true))
            {
                return;
            }
            string sanitizedId = DatabaseEssentials.Security.Sanitize(server.Account.Id);
            string deleteCookies = "DELETE FROM Tbl_cookies WHERE userid = " + sanitizedId + ";";
            string deleteAdmin = "DELETE FROM Tbl_admin WHERE userid = " + sanitizedId + ";";
            string deleteEvent = "DELETE FROM Tbl_event WHERE userid = " + sanitizedId + ";";
            string deleteLog = "DELETE FROM Tbl_log WHERE userid = " + sanitizedId + ";";
            string deleteLikes = "DELETE FROM Tbl_likes WHERE sourceid = " + sanitizedId + " OR targetid = " + sanitizedId + ";";
            string deleteDislikes = "DELETE FROM Tbl_dislikes WHERE sourceid = " + sanitizedId + " OR targetid = " + sanitizedId + ";";
            string deleteMatches = "DELETE FROM Tbl_match WHERE userid1 = " + sanitizedId + " OR userid2 = " + sanitizedId + ";";
            string query = deleteCookies + deleteAdmin + deleteEvent + deleteLog + deleteLikes + deleteDislikes + deleteMatches;
            SqlApiRequest sqlRequest = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optional = await databaseManager.GetModifyDataResponseAsync(sqlRequest);
            if (!optional.Success)
            {
                return;
            }
            GenericSuccessResponse response = new GenericSuccessResponse(ResponseId.DeleteAccount, true);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(response);
            string json = serializedApiResponse.Serialize();
            server.Send(json);
            server.UnitTesting.MethodSuccess = true;
        }
    }
}