using Newtonsoft.Json;
using System.Diagnostics;

namespace qsrv.ApiResponses
{
    /// <summary>
    /// Api response base class
    /// </summary>
    public abstract class ApiResponse
    {
        public ResponseId ResponseId;

        /// <summary>
        /// Serializes the current object to a JSON string.
        /// </summary>
        /// <returns>The current object as a JSON string.</returns>
        public virtual string Serialize()
        {
            string json = JsonConvert.SerializeObject(this);
            Debug.WriteLine("<< " + json);
            return json;
        }
    }
}