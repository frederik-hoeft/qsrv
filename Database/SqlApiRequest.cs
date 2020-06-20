using Newtonsoft.Json;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiRequests;

namespace qsrv.Database
{
    /// <summary>
    /// SQL API Request wrapper class. Can hold any SQL API Request.
    /// </summary>
    public sealed class SqlApiRequest : ApiRequestBase
    {
        private SqlApiRequest(SqlRequestId sqlRequestId, string query, int expectedColumns)
        {
            RequestId = sqlRequestId;
            Query = query;
            ExpectedColumns = expectedColumns;
        }

        public static SqlApiRequest Create(SqlRequestId sqlRequestId, string query, int expectedColumns)
        {
            return new SqlApiRequest(sqlRequestId, query, expectedColumns);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}