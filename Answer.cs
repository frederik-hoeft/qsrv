using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv
{
    public class Answer
    {
        public string Text { get; } = string.Empty;
        public bool IsCorrect { get; }
        public Answer(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;
        }
    }
}
