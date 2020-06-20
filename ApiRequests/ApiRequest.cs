namespace qsrv.ApiRequests
{
    /// <summary>
    /// Api request base class
    /// </summary>
    public abstract class ApiRequest
    {
        public ApiRequestId RequestId;

        public abstract void Process(ApiServer server);
    }
}