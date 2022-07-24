using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class GradeDAO
    {
        public GradeDAO()
        {
            Exams = new HashSet<ExamDAO>();
            Questions = new HashSet<QuestionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ExamDAO> Exams { get; set; }
        public virtual ICollection<QuestionDAO> Questions { get; set; }
    }
}
