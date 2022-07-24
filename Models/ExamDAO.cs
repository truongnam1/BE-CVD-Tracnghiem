using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ExamDAO
    {
        public ExamDAO()
        {
            ExamHistories = new HashSet<ExamHistoryDAO>();
            ExamQuestionMappings = new HashSet<ExamQuestionMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SubjectId { get; set; }
        public long ExamLevelId { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public long GradeId { get; set; }
        public long ExamStatusId { get; set; }
        public decimal? TotalMark { get; set; }
        public long TotalQuestion { get; set; }
        public long? ImageId { get; set; }
        public long? Time { get; set; }

        public virtual AppUserDAO Creator { get; set; }
        public virtual ExamLevelDAO ExamLevel { get; set; }
        public virtual ExamStatusDAO ExamStatus { get; set; }
        public virtual GradeDAO Grade { get; set; }
        public virtual ImageDAO Image { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual SubjectDAO Subject { get; set; }
        public virtual ICollection<ExamHistoryDAO> ExamHistories { get; set; }
        public virtual ICollection<ExamQuestionMappingDAO> ExamQuestionMappings { get; set; }
    }
}
