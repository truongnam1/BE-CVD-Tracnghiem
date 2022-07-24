using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class FieldTypeDAO
    {
        public FieldTypeDAO()
        {
            Fields = new HashSet<FieldDAO>();
            PermissionOperators = new HashSet<PermissionOperatorDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FieldDAO> Fields { get; set; }
        public virtual ICollection<PermissionOperatorDAO> PermissionOperators { get; set; }
    }
}
