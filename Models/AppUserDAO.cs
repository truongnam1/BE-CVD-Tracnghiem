using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            AppUserRoleMappings = new HashSet<AppUserRoleMappingDAO>();
            ExamHistories = new HashSet<ExamHistoryDAO>();
            Exams = new HashSet<ExamDAO>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public long? ImageId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long StatusId { get; set; }

        public virtual ImageDAO Image { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserRoleMappingDAO> AppUserRoleMappings { get; set; }
        public virtual ICollection<ExamHistoryDAO> ExamHistories { get; set; }
        public virtual ICollection<ExamDAO> Exams { get; set; }
    }
}
