﻿using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ExamLevelDAO
    {
        public ExamLevelDAO()
        {
            Exams = new HashSet<ExamDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ExamDAO> Exams { get; set; }
    }
}
