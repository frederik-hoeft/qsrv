using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv.ApiRequests
{
    public class GetHighscoresRequest : ApiRequest
    {
        public int Count { get; }
        public GetHighscoresRequest(int count)
        {
            Count = count;
        }
        public override void Process(ApiServer server)
        {
            throw new NotImplementedException();
        }
    }
}
