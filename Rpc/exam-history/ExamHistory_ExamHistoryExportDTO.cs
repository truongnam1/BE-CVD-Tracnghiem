using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam_history
{
    public class ExamHistory_ExamHistoryExportDTO : DataDTO
    {
        public long STT {get; set; }
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ExamId { get; set; }
        public long Times { get; set; }
        public long CorrectAnswerQuantity { get; set; }
        public long TotalQuestionQuantity { get; set; }
        public decimal Mark { get; set; }
        public DateTime ExamedAt { get; set; }
        public ExamHistory_AppUserDTO AppUser { get; set; }
        public ExamHistory_ExamDTO Exam { get; set; }
        public ExamHistory_ExamHistoryExportDTO() {}
        public ExamHistory_ExamHistoryExportDTO(ExamHistory ExamHistory)
        {
            this.Id = ExamHistory.Id;
            this.AppUserId = ExamHistory.AppUserId;
            this.ExamId = ExamHistory.ExamId;
            this.Times = ExamHistory.Times;
            this.CorrectAnswerQuantity = ExamHistory.CorrectAnswerQuantity;
            this.TotalQuestionQuantity = ExamHistory.TotalQuestionQuantity;
            this.Mark = ExamHistory.Mark;
            this.ExamedAt = ExamHistory.ExamedAt;
            this.AppUser = ExamHistory.AppUser == null ? null : new ExamHistory_AppUserDTO(ExamHistory.AppUser);
            this.Exam = ExamHistory.Exam == null ? null : new ExamHistory_ExamDTO(ExamHistory.Exam);
            this.Informations = ExamHistory.Informations;
            this.Warnings = ExamHistory.Warnings;
            this.Errors = ExamHistory.Errors;
        }
    }
}
