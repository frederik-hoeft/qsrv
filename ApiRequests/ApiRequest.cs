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

        public virtual void Process(ApiServer server)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.Reset();
            ProcessAsync(server, manualResetEvent);
            manualResetEvent.WaitOne(Timeout.Infinite);
        }

        private protected abstract void ProcessAsync(ApiServer server, ManualResetEvent manualResetEvent);
    }
}