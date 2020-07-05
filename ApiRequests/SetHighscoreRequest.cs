using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv.ApiRequests
{
    public class SetHighscoreRequest : ApiRequest
    {
        public string Name { get; } = string.Empty;
        public int Score { get; } = -1;
        public SetHighscoreRequest(string name, int score)
        {
            Name = name;
            Score = score;
        }
        public override void Process(ApiServer server)
        {
            throw new NotImplementedException();
        }
    }
}
