using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv.ApiResponses
{
    public class GetHighscoresResponse : ApiResponse
    {
        public readonly Highscore[] Highscores;
        public readonly int Count;
        public GetHighscoresResponse(Highscore[] highscores)
        {
            ResponseId = ResponseId.GetHighscores;
            Highscores = highscores;
            Count = highscores.Length;
        }
    }
}
