using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ExamHistoryDAO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long ExamId { get; set; }
        public long Times { get; set; }
        public long CorrectAnswerQuantity { get; set; }
        public long TotalQuestionQuantity { get; set; }
        public decimal Mark { get; set; }
        public DateTime ExamedAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual ExamDAO Exam { get; set; }
    }
}
