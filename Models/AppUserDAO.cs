using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            ExamHistories = new HashSet<ExamHistoryDAO>();
            Exams = new HashSet<ExamDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public long RoleId { get; set; }
        public long? ImageId { get; set; }

        public virtual ImageDAO Image { get; set; }
        public virtual RoleDAO Role { get; set; }
        public virtual ICollection<ExamHistoryDAO> ExamHistories { get; set; }
        public virtual ICollection<ExamDAO> Exams { get; set; }
    }
}
