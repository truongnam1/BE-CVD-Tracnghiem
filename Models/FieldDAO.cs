using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class FieldDAO
    {
        public FieldDAO()
        {
            PermissionContents = new HashSet<PermissionContentDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long FieldTypeId { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual FieldTypeDAO FieldType { get; set; }
        public virtual MenuDAO Menu { get; set; }
        public virtual ICollection<PermissionContentDAO> PermissionContents { get; set; }
    }
}
