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
            MainServer.Run();
        }
    }
}