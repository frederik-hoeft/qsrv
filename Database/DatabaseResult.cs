using System;
using System.Collections.Generic;
using System.Text;
using washared.DatabaseServer.ApiResponses;

namespace qsrv.Database
{
    public class DatabaseResult
    {
        public ApiResponse ApiResponse { get; }
        public bool Success { get; }
        public DatabaseResult(ApiResponse apiResponse, bool success)
        {
            ApiResponse = apiResponse;
            Success = success;
        }
    }
}
