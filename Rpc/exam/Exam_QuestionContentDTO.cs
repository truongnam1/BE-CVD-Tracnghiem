using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam
{
    public class Exam_QuestionContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string AnswerContent { get; set; }
        public bool IsRight { get; set; }
        public Exam_QuestionContentDTO() { }
        public Exam_QuestionContentDTO(QuestionContent QuestionContent)
        {
            this.Id = QuestionContent.Id;
            this.QuestionId = QuestionContent.QuestionId;
            this.AnswerContent = QuestionContent.AnswerContent;
            this.IsRight = QuestionContent.IsRight;
            this.Errors = QuestionContent.Errors;
        }
    }

    public class Exam_QuestionContentFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter QuestionId { get; set; }
        public StringFilter AnswerContent { get; set; }
        public QuestionContentOrder OrderBy { get; set; }
    }
}