using TrueSight.Common;
using Tracnghiem.Common;
using System;

namespace Tracnghiem.Entities
{
    public class AuditLog : DataEntity
    {
        public long Id { get; set; }
        public long? AppUserId { get; set; }
        public string AppUser { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string ModuleName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }
    }
}
