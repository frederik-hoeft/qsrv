using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv
{
    public class Highscore
    {
        public string Player { get; } = string.Empty;
        public int Score { get; } = -1;
        public Highscore(string player, int score)
        {
            Player = player;
            Score = score;
        }
    }
}
