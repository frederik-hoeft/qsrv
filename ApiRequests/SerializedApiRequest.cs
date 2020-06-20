using Newtonsoft.Json;

namespace qsrv.ApiRequests
{
    /// <summary>
    /// Api request serialization wrapper class
    /// </summary>
    public class SerializedApiRequest
    {
        public readonly ApiRequestId ApiRequestId;
        public readonly string Json;

        public SerializedApiRequest(ApiRequestId apiRequestId, string json)
        {
            ApiRequestId = apiRequestId;
            Json = json;
        }

        public ApiRequest Deserialize()
        {
            return ApiRequestId switch
            {

                _ => null
            };
        }
    }

    public enum ApiRequestId
    {
        Invalid = -1,
        CookieValidation = 0,
        BatchProfile = 1,
        CreateEventA = 2,
        DeleteEventA = 3,
        EditEventA = 4,
        GetEventInfo = 5,
        GetEventA = 6,
        HandleDislikeEvent = 7,
        HandleLikeEvent = 8,
        CreateCookie = 9,
        CreateAccount = 10,
        ConfirmAccount = 11,
        GetAccountInfo = 12,
        UpdateAccountInfo = 13,
        PasswordChange = 14,
        ConfirmPasswordChange = 15,
        PasswordReset = 16,
        ConfirmPasswordReset = 17,
        GetAllEvents = 18,
        ChangeUserPermissionsA = 19,
        DeleteAccount = 20,
    }
}