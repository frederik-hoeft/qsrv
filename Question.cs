using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv
{
    public class Question
    {
        internal string Id { get; } = string.Empty;
        public string Text { get; } = string.Empty;
        public Answer[] Answers { get; }
        public Category Category { get; }
        public Question(string id, string text, Answer[] answers, Category category)
        {
            Id = id;
            Text = text;
            Answers = answers;
            Category = category;
        }

        public Question(string id, string text, Category category)
        {
            Id = id;
            Text = text;
            Answers = new Answer[4];
            Category = category;
        }
    }
    public enum Category
    {
        Undefined = -1,
        Psychology = 1,
    }
}
