using qsrv.ApiRequests;
using qsrv.ApiResponses;

namespace qsrv
{
    public class UnitTest
    {
        public bool MethodSuccess { get; set; } = false;
        public ApiErrorCode ErrorCode { get; set; } = ApiErrorCode.InternalServerError;
    }
}