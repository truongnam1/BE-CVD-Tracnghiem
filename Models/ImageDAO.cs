using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ImageDAO
    {
        public ImageDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
            Exams = new HashSet<ExamDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<ExamDAO> Exams { get; set; }
    }
}
