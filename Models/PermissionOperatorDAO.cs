using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class PermissionOperatorDAO
    {
        public PermissionOperatorDAO()
        {
            PermissionContents = new HashSet<PermissionContentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long FieldTypeId { get; set; }

        public virtual FieldTypeDAO FieldType { get; set; }
        public virtual ICollection<PermissionContentDAO> PermissionContents { get; set; }
    }
}
