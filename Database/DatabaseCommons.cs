using System;
using System.Text;
using System.Threading.Tasks;
using qsrv.ApiResponses;
using qsrv.Security;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.Database
{
    public sealed partial class DatabaseManager
    {
        public async Task<bool> SetupAccount(string id)
        {
            if (server.Account == null)
            {
                server.Account = null; // TODO!!!
                // if (!success)
                // {
                //     return false;
                // }
            }
            return await SetUserOnline();
        }

        public async Task<bool> SetUserOnline()
        {
            if (server.AssertIdSet())
            {
                return false;
            }
            string query = "UPDATE Tbl_user SET isOnline = 1 WHERE id = " + server.Account.Id;
            SqlApiRequest sqlRequest = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optional = await GetModifyDataResponseAsync(sqlRequest);
            if (optional.Success && optional.Result.Success)
            {
                server.Account.IsOnline = true;
            }
            return optional.Success && optional.Result.Success;
        }

        public async Task<bool> SetUserOffline()
        {
            if (server.AssertIdSet())
            {
                return false;
            }
            string query = "UPDATE Tbl_user SET isOnline = 0 WHERE id = " + server.Account.Id;
            SqlApiRequest sqlRequest = SqlApiRequest.Create(SqlRequestId.ModifyData, query, -1);
            Optional<SqlModifyDataResponse> optional = await GetModifyDataResponseAsync(sqlRequest);
            if (optional.Success && optional.Result.Success)
            {
                server.Account.IsOnline = false;
            }
            return optional.Success && optional.Result.Success;
        }

        public async Task<Optional<bool>> CheckUserExists(string userId)
        {
            string query = "SELECT 1 FROM Tbl_user WHERE hid = \'" + DatabaseEssentials.Security.Sanitize(userId) + "\' LIMIT 1;";
            SqlApiRequest sqlRequest = SqlApiRequest.Create(SqlRequestId.GetSingleOrDefault, query, 1);
            Optional<SqlSingleOrDefaultResponse> optional = await GetSingleOrDefaultResponseAsync(sqlRequest);
            if (!optional.Success)
            {
                return new Optional<bool>(false, false);
            }
            return new Optional<bool>(optional.Result.Success, true);
        }

        public async Task<bool> OptionalAssertUserExists(string id, bool isDatabaseId)
        {
            if (MainServer.Config.AdvancedErrorChecking)
            {
                string query;
                if (isDatabaseId)
                {
                    query = "SELECT 1 FROM Tbl_user WHERE id = " + DatabaseEssentials.Security.Sanitize(id) + ";";
                }
                else
                {
                    query = "SELECT 1 FROM Tbl_user WHERE hid = \'" + DatabaseEssentials.Security.Sanitize(id) + "\';";
                }
                SqlApiRequest sqlRequest = SqlApiRequest.Create(SqlRequestId.GetSingleOrDefault, query, 1);
                Optional<SqlSingleOrDefaultResponse> optional = await GetSingleOrDefaultResponseAsync(sqlRequest);
                if (!optional.Success)
                {
                    return true;
                }
                return !optional.Result.Success;
            }
            return false;
        }
    }
}