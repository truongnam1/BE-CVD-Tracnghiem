using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam_history
{
    public class ExamHistory_ExamDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SubjectId { get; set; }
        public long ExamLevelId { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public long GradeId { get; set; }
        public long ExamStatusId { get; set; }
        public decimal? TotalMark { get; set; }
        public long TotalQuestion { get; set; }
        public long? ImageId { get; set; }
        public long? Time { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ExamHistory_ExamDTO() {}
        public ExamHistory_ExamDTO(Exam Exam)
        {
            this.Id = Exam.Id;
            this.Code = Exam.Code;
            this.Name = Exam.Name;
            this.SubjectId = Exam.SubjectId;
            this.ExamLevelId = Exam.ExamLevelId;
            this.StatusId = Exam.StatusId;
            this.CreatorId = Exam.CreatorId;
            this.GradeId = Exam.GradeId;
            this.ExamStatusId = Exam.ExamStatusId;
            this.TotalMark = Exam.TotalMark;
            this.TotalQuestion = Exam.TotalQuestion;
            this.ImageId = Exam.ImageId;
            this.Time = Exam.Time;
            this.RowId = Exam.RowId;
            this.CreatedAt = Exam.CreatedAt;
            this.UpdatedAt = Exam.UpdatedAt;
            this.Informations = Exam.Informations;
            this.Warnings = Exam.Warnings;
            this.Errors = Exam.Errors;
        }
    }

    public class ExamHistory_ExamFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter SubjectId { get; set; }
        public IdFilter ExamLevelId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter GradeId { get; set; }
        public IdFilter ExamStatusId { get; set; }
        public DecimalFilter TotalMark { get; set; }
        public LongFilter TotalQuestion { get; set; }
        public IdFilter ImageId { get; set; }
        public LongFilter Time { get; set; }
        public string Search { get; set; }
        public ExamOrder OrderBy { get; set; }
    }
}