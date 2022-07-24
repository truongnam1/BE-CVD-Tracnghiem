using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.question
{
    public class Question_QuestionDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SubjectId { get; set; }
        public long QuestionGroupId { get; set; }
        public long QuestionTypeId { get; set; }
        public string Content { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public long GradeId { get; set; }
        public Question_GradeDTO Grade { get; set; }
        public Question_QuestionGroupDTO QuestionGroup { get; set; }
        public Question_QuestionTypeDTO QuestionType { get; set; }
        public Question_StatusDTO Status { get; set; }
        public Question_SubjectDTO Subject { get; set; }
        public List<Question_QuestionContentDTO> QuestionContents { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Question_QuestionDTO() {}
        public Question_QuestionDTO(Question Question)
        {
            this.Id = Question.Id;
            this.Code = Question.Code;
            this.Name = Question.Name;
            this.SubjectId = Question.SubjectId;
            this.QuestionGroupId = Question.QuestionGroupId;
            this.QuestionTypeId = Question.QuestionTypeId;
            this.Content = Question.Content;
            this.StatusId = Question.StatusId;
            this.CreatorId = Question.CreatorId;
            this.GradeId = Question.GradeId;
            this.Grade = Question.Grade == null ? null : new Question_GradeDTO(Question.Grade);
            this.QuestionGroup = Question.QuestionGroup == null ? null : new Question_QuestionGroupDTO(Question.QuestionGroup);
            this.QuestionType = Question.QuestionType == null ? null : new Question_QuestionTypeDTO(Question.QuestionType);
            this.Status = Question.Status == null ? null : new Question_StatusDTO(Question.Status);
            this.Subject = Question.Subject == null ? null : new Question_SubjectDTO(Question.Subject);
            this.QuestionContents = Question.QuestionContents?.Select(x => new Question_QuestionContentDTO(x)).ToList();
            this.RowId = Question.RowId;
            this.CreatedAt = Question.CreatedAt;
            this.UpdatedAt = Question.UpdatedAt;
            this.Informations = Question.Informations;
            this.Warnings = Question.Warnings;
            this.Errors = Question.Errors;
        }
    }

    public class Question_QuestionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter SubjectId { get; set; }
        public IdFilter QuestionGroupId { get; set; }
        public IdFilter QuestionTypeId { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter GradeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public string Search { get; set; }
        public QuestionOrder OrderBy { get; set; }
    }
}
