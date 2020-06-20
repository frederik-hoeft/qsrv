namespace qsrv.ApiResponses
{
    public class GenericSuccessResponse : ApiResponse
    {
        public readonly bool Success;

        public GenericSuccessResponse(ResponseId responseId, bool success)
        {
            ResponseId = responseId;
            Success = success;
        }
    }
}