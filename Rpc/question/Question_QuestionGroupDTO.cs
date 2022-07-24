using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.question
{
    public class Question_QuestionGroupDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Question_QuestionGroupDTO() {}
        public Question_QuestionGroupDTO(QuestionGroup QuestionGroup)
        {
            this.Id = QuestionGroup.Id;
            this.Code = QuestionGroup.Code;
            this.Name = QuestionGroup.Name;
            this.Informations = QuestionGroup.Informations;
            this.Warnings = QuestionGroup.Warnings;
            this.Errors = QuestionGroup.Errors;
        }
    }

    public class Question_QuestionGroupFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public QuestionGroupOrder OrderBy { get; set; }
    }
}