using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class QuestionDAO
    {
        public QuestionDAO()
        {
            ExamQuestionMappings = new HashSet<ExamQuestionMappingDAO>();
            QuestionContents = new HashSet<QuestionContentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SubjectId { get; set; }
        public long QuestionGroupId { get; set; }
        public long QuestionTypeId { get; set; }
        public string Content { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public long GradeId { get; set; }

        public virtual GradeDAO Grade { get; set; }
        public virtual QuestionGroupDAO QuestionGroup { get; set; }
        public virtual QuestionTypeDAO QuestionType { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual SubjectDAO Subject { get; set; }
        public virtual ICollection<ExamQuestionMappingDAO> ExamQuestionMappings { get; set; }
        public virtual ICollection<QuestionContentDAO> QuestionContents { get; set; }
    }
}
