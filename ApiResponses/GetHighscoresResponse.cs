using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv.ApiResponses
{
    public class GetHighscoresResponse : ApiResponse
    {
        public Highscore[] Highscores { get; }
        public int Count { get; }
        public GetHighscoresResponse(Highscore[] highscores)
        {
            ResponseId = ResponseId.GetHighscores;
            Highscores = highscores;
            Count = highscores.Length;
        }
    }
}
