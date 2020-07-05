using Newtonsoft.Json;
using qsrv.ApiResponses;
using System.Reflection.Metadata.Ecma335;

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
                ApiRequestId.GetQuestions => JsonConvert.DeserializeObject<GetQuestionsRequest>(Json),
                ApiRequestId.GetHighscores => JsonConvert.DeserializeObject<GetHighscoresRequest>(Json),
                ApiRequestId.SetHighscore => JsonConvert.DeserializeObject<SetHighscoreRequest>(Json),
                _ => null
            };
        }
    }

    public enum ApiRequestId
    {
        Invalid = -1,
        GetQuestions = 0,
        GetHighscores = 1,
        SetHighscore = 2
    }
}