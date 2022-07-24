using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam
{
    public class Exam_ExamExportDTO : DataDTO
    {
        public long STT {get; set; }
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
        public Exam_AppUserDTO Creator { get; set; }
        public Exam_ExamLevelDTO ExamLevel { get; set; }
        public Exam_ExamStatusDTO ExamStatus { get; set; }
        public Exam_GradeDTO Grade { get; set; }
        public Exam_ImageDTO Image { get; set; }
        public Exam_StatusDTO Status { get; set; }
        public Exam_SubjectDTO Subject { get; set; }
        public List<Exam_ExamQuestionMappingDTO> ExamQuestionMappings { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Exam_ExamExportDTO() {}
        public Exam_ExamExportDTO(Exam Exam)
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
            this.Creator = Exam.Creator == null ? null : new Exam_AppUserDTO(Exam.Creator);
            this.ExamLevel = Exam.ExamLevel == null ? null : new Exam_ExamLevelDTO(Exam.ExamLevel);
            this.ExamStatus = Exam.ExamStatus == null ? null : new Exam_ExamStatusDTO(Exam.ExamStatus);
            this.Grade = Exam.Grade == null ? null : new Exam_GradeDTO(Exam.Grade);
            this.Image = Exam.Image == null ? null : new Exam_ImageDTO(Exam.Image);
            this.Status = Exam.Status == null ? null : new Exam_StatusDTO(Exam.Status);
            this.Subject = Exam.Subject == null ? null : new Exam_SubjectDTO(Exam.Subject);
            this.ExamQuestionMappings = Exam.ExamQuestionMappings?.Select(x => new Exam_ExamQuestionMappingDTO(x)).ToList();
            this.RowId = Exam.RowId;
            this.CreatedAt = Exam.CreatedAt;
            this.UpdatedAt = Exam.UpdatedAt;
            this.Informations = Exam.Informations;
            this.Warnings = Exam.Warnings;
            this.Errors = Exam.Errors;
        }
    }
}
