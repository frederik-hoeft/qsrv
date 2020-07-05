using Newtonsoft.Json;

namespace qsrv.ApiResponses
{
    /// <summary>
    /// API response serialization wrapper class
    /// </summary>
    public sealed class SerializedApiResponse
    {
        public readonly ResponseId ResponseId;
        public readonly string Json;

        private SerializedApiResponse(ResponseId responseId, string json)
        {
            ResponseId = responseId;
            Json = json;
        }

        public static SerializedApiResponse Create(ResponseId responseId, string json)
        {
            return new SerializedApiResponse(responseId, json);
        }

        public static SerializedApiResponse Create(ApiResponse apiResponse)
        {
            return new SerializedApiResponse(apiResponse.ResponseId, apiResponse.Serialize());
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum ResponseId
    {
        Error = -1,
        GetQuestions = 0,
        GetHighscores = 1,
        SetHighscore = 2
    }
}