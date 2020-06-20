using System;
using System.Diagnostics;
using System.Threading;
using qsrv.ApiRequests;
using qsrv.Database;
using washared.DatabaseServer;
using washared.DatabaseServer.ApiResponses;

namespace qsrv
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        private static void Main()
        {
            MainServer.LoadConfig();
        }

        private async static void PerformanceTest()
        {
            ApiServer server = ApiServer.CreateDummy();
            using DatabaseManager databaseManager = new DatabaseManager(server);
            const string query = "SELECT id FROM Tbl_user WHERE id = 1 LIMIT 1";
            SqlApiRequest request = SqlApiRequest.Create(SqlRequestId.GetSingleOrDefault, query, 1);
            const int maxIter = 10000;
            Console.WriteLine("Performance Test: Sending " + maxIter.ToString() + " requests to DB server ...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < maxIter; i++)
            {
                Optional<SqlSingleOrDefaultResponse> optional = await databaseManager.GetSingleOrDefaultResponseAsync(request);
                if (!optional.Success)
                {
                    break;
                }
                Console.WriteLine("Request #" + i.ToString() + " succeeded!");
            }
            stopwatch.Stop();
            double average = stopwatch.ElapsedMilliseconds / (double)maxIter;
            Console.WriteLine("Average time per request: " + average.ToString() + " ms.");
        }
    }
}