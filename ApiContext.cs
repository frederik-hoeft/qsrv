using qsrv.ApiRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv
{
    public class ApiContext
    {
        public ApiServer Server { get; set; }
        public ApiRequestId RequestId { get; set; }
        public UnitTest UnitTest { get; } = new UnitTest();
        public ApiContext(ApiServer server, ApiRequestId requestId)
        {
            RequestId = requestId;
            Server = server;
        }

        public ApiContext(ApiServer server)
        {
            Server = server;
        }

        public ApiContext(ApiRequestId requestId)
        {
            RequestId = requestId;
        }
    }
}
