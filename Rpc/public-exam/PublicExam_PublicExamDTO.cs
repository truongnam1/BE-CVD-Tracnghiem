using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.public_exam
{
    public class PublicExam_PublicExamDTO : DataDTO
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
        public long CurrentMonthNumberTest { get; set; }
        public long TotalNumberTest { get; set; }
        public PublicExam_AppUserDTO Creator { get; set; }
        public PublicExam_ExamLevelDTO ExamLevel { get; set; }
        public PublicExam_ExamStatusDTO ExamStatus { get; set; }
        public PublicExam_GradeDTO Grade { get; set; }
        public PublicExam_ImageDTO Image { get; set; }
        public PublicExam_StatusDTO Status { get; set; }
        public PublicExam_SubjectDTO Subject { get; set; }
        public List<PublicExam_ExamQuestionMappingDTO> ExamQuestionMappings { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PublicExam_PublicExamDTO() {}
        public PublicExam_PublicExamDTO(Exam Exam)
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
            this.CurrentMonthNumberTest = Exam.CurrentMonthNumberTest;
            this.TotalNumberTest = Exam.TotalNumberTest;
            this.Creator = Exam.Creator == null ? null : new PublicExam_AppUserDTO(Exam.Creator);
            this.ExamLevel = Exam.ExamLevel == null ? null : new PublicExam_ExamLevelDTO(Exam.ExamLevel);
            this.ExamStatus = Exam.ExamStatus == null ? null : new PublicExam_ExamStatusDTO(Exam.ExamStatus);
            this.Grade = Exam.Grade == null ? null : new PublicExam_GradeDTO(Exam.Grade);
            this.Image = Exam.Image == null ? null : new PublicExam_ImageDTO(Exam.Image);
            this.Status = Exam.Status == null ? null : new PublicExam_StatusDTO(Exam.Status);
            this.Subject = Exam.Subject == null ? null : new PublicExam_SubjectDTO(Exam.Subject);
            this.ExamQuestionMappings = Exam.ExamQuestionMappings?.Select(x => new PublicExam_ExamQuestionMappingDTO(x)).ToList();
            this.RowId = Exam.RowId;
            this.CreatedAt = Exam.CreatedAt;
            this.UpdatedAt = Exam.UpdatedAt;
            this.Informations = Exam.Informations;
            this.Warnings = Exam.Warnings;
            this.Errors = Exam.Errors;
        }
    }

    public class PublicExam_PublicExamFilterDTO : FilterDTO
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
        public LongFilter CurrentMonthNumberTest { get; set; }
        public LongFilter TotalNumberTest { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public string Search { get; set; }
        public ExamOrder OrderBy { get; set; }
    }
}
