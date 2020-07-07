using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv
{
    public class Question
    {
        internal readonly string Id = string.Empty;
        public readonly string Text = string.Empty;
        public readonly Answer[] Answers;
        public readonly Category Category;

        internal Question(string text, Answer[] answers, Category category)
        {
            Text = text;
            Answers = answers;
            Category = category;
        }

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
        Science = 2,
        Everyday = 3,
        Nerdistan = 4,
    }
}
