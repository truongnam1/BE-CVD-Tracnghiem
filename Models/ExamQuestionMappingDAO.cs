using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ExamQuestionMappingDAO
    {
        public long ExamId { get; set; }
        public long QuestionId { get; set; }
        public decimal? Mark { get; set; }

        public virtual ExamDAO Exam { get; set; }
        public virtual QuestionDAO Question { get; set; }
    }
}
