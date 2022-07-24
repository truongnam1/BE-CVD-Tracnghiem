using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class PermissionContentDAO
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public long FieldId { get; set; }
        public long PermissionOperatorId { get; set; }
        public string Value { get; set; }

        public virtual FieldDAO Field { get; set; }
        public virtual PermissionDAO Permission { get; set; }
        public virtual PermissionOperatorDAO PermissionOperator { get; set; }
    }
}
