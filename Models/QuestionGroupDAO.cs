using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class QuestionGroupDAO
    {
        public QuestionGroupDAO()
        {
            Questions = new HashSet<QuestionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<QuestionDAO> Questions { get; set; }
    }
}
