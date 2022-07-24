using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.question
{
    public class Question_QuestionTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Question_QuestionTypeDTO() {}
        public Question_QuestionTypeDTO(QuestionType QuestionType)
        {
            this.Id = QuestionType.Id;
            this.Code = QuestionType.Code;
            this.Name = QuestionType.Name;
            this.Informations = QuestionType.Informations;
            this.Warnings = QuestionType.Warnings;
            this.Errors = QuestionType.Errors;
        }
    }

    public class Question_QuestionTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public QuestionTypeOrder OrderBy { get; set; }
    }
}