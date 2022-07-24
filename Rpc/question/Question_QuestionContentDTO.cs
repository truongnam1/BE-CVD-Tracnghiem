using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.question
{
    public class Question_QuestionContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string AnswerContent { get; set; }
        public bool IsRight { get; set; }
        public Question_QuestionContentDTO() {}
        public Question_QuestionContentDTO(QuestionContent QuestionContent)
        {
            this.Id = QuestionContent.Id;
            this.QuestionId = QuestionContent.QuestionId;
            this.AnswerContent = QuestionContent.AnswerContent;
            this.IsRight = QuestionContent.IsRight;
            this.Errors = QuestionContent.Errors;
        }
    }

    public class Question_QuestionContentFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter QuestionId { get; set; }
        public StringFilter AnswerContent { get; set; }
        public QuestionContentOrder OrderBy { get; set; }
    }
}