using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv.ApiResponses
{
    public class GetQuestionsResponse : ApiResponse
    {
        public readonly Question[] Questions;
        public readonly int Count;
        public GetQuestionsResponse(Question[] questions)
        {
            ResponseId = ResponseId.GetQuestions;
            Questions = questions;
            Count = questions?.Length ?? -1;
        }
    }
}
