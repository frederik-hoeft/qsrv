using qsrv.ApiResponses;
using qsrv.Database;
using qsrv.Security;

namespace qsrv.ApiRequests
{
    public class CreateAccountRequest : ApiRequest
    {
        public readonly string Username;
        private readonly string Password;

        public CreateAccountRequest(ApiRequestId requestId, string username, string password)
        {
            RequestId = requestId;
            Username = username;
            Password = password;
        }

        public override void Process(ApiServer server)
        {
            if (server.AssertServerSetup(this) || server.AssertAccountNull())
            {
                return;
            }
            string passwordHash = SecurityManager.ScryptHash(Password);
            server.Account = null; // TODO!!
            GenericSuccessResponse apiResponse = new GenericSuccessResponse(ResponseId.CreateAccount, true);
            SerializedApiResponse serializedApiResponse = SerializedApiResponse.Create(apiResponse);
            string json = serializedApiResponse.Serialize();
            server.Send(json);
            server.UnitTesting.MethodSuccess = true;
        }
    }
}