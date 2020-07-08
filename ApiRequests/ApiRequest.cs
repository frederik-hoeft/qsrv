using qsrv.ApiResponses;
using System.Threading;
using System.Threading.Tasks;

namespace qsrv.ApiRequests
{
    /// <summary>
    /// Api request base class
    /// </summary>
    public abstract class ApiRequest
    {
        public ApiRequestId RequestId;
        public ApiContext Context { get; private protected set; }

        private protected bool isStale = false;

        private protected ApiRequest(ApiRequestId requestId)
        {
            Context = new ApiContext(requestId);
        }

        public virtual void Process(ApiServer server)
        {
            Context.Server = server;
            if (isStale)
            {
                ApiError.Throw(ApiErrorCode.InternalServerError, Context, "Stale Request: tried to process same request more than once.");
                return;
            }
            ManualResetEventSlim manualResetEvent;
            if (server.IsSynchonous)
            {
                manualResetEvent = new ManualResetEventSlim(false);
                manualResetEvent.Reset();
            }
            else
            {
                manualResetEvent = null;
            }
            ProcessAsync(server, manualResetEvent);
            manualResetEvent?.Wait(Timeout.Infinite);
            isStale = true;
        }

        private protected abstract void ProcessAsync(ApiServer server, ManualResetEventSlim manualResetEvent);
    }
}