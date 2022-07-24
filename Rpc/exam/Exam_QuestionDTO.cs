using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam
{
    public class Exam_QuestionDTO : DataDTO
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
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Exam_QuestionDTO() {}
        public Exam_QuestionDTO(Question Question)
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
            this.RowId = Question.RowId;
            this.CreatedAt = Question.CreatedAt;
            this.UpdatedAt = Question.UpdatedAt;
            this.Informations = Question.Informations;
            this.Warnings = Question.Warnings;
            this.Errors = Question.Errors;
        }
    }

    public class Exam_QuestionFilterDTO : FilterDTO
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
        public string Search { get; set; }
        public QuestionOrder OrderBy { get; set; }
    }
}