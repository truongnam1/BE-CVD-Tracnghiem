using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class QuestionContentDAO
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string AnswerContent { get; set; }
        public bool IsRight { get; set; }

        public virtual QuestionDAO Question { get; set; }
    }
}
