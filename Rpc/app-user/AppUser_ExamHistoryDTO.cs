using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_ExamHistoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ExamId { get; set; }
        public long Times { get; set; }
        public long CorrectAnswerQuantity { get; set; }
        public long TotalQuestionQuantity { get; set; }
        public decimal Mark { get; set; }
        public DateTime ExamedAt { get; set; }
        public AppUser_ExamDTO Exam { get; set; }
        public AppUser_ExamHistoryDTO() {}
        public AppUser_ExamHistoryDTO(ExamHistory ExamHistory)
        {
            this.Id = ExamHistory.Id;
            this.AppUserId = ExamHistory.AppUserId;
            this.ExamId = ExamHistory.ExamId;
            this.Times = ExamHistory.Times;
            this.CorrectAnswerQuantity = ExamHistory.CorrectAnswerQuantity;
            this.TotalQuestionQuantity = ExamHistory.TotalQuestionQuantity;
            this.Mark = ExamHistory.Mark;
            this.ExamedAt = ExamHistory.ExamedAt;
            this.Exam = ExamHistory.Exam == null ? null : new AppUser_ExamDTO(ExamHistory.Exam);
            this.Errors = ExamHistory.Errors;
        }
    }

    public class AppUser_ExamHistoryFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter ExamId { get; set; }
        public LongFilter Times { get; set; }
        public LongFilter CorrectAnswerQuantity { get; set; }
        public LongFilter TotalQuestionQuantity { get; set; }
        public DecimalFilter Mark { get; set; }
        public DateFilter ExamedAt { get; set; }
        public ExamHistoryOrder OrderBy { get; set; }
    }
}