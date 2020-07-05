using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv.ApiResponses
{
    public class GetQuestionsResponse : ApiResponse
    {
        public Question[] Questions { get; }
        public int Count { get; }
        public GetQuestionsResponse(Question[] questions)
        {
            ResponseId = ResponseId.GetQuestions;
            Questions = questions;
            Count = questions.Length;
        }
    }
}
