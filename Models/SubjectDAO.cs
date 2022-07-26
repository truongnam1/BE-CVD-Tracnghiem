﻿using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class SubjectDAO
    {
        public SubjectDAO()
        {
            Exams = new HashSet<ExamDAO>();
            Questions = new HashSet<QuestionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

        public virtual ICollection<ExamDAO> Exams { get; set; }
        public virtual ICollection<QuestionDAO> Questions { get; set; }
    }
}
