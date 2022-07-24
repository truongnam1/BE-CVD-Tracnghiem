using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class MenuDAO
    {
        public MenuDAO()
        {
            Actions = new HashSet<ActionDAO>();
            Fields = new HashSet<FieldDAO>();
            Permissions = new HashSet<PermissionDAO>();
        }

        public long Id { get; set; }
        public long SiteId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ActionDAO> Actions { get; set; }
        public virtual ICollection<FieldDAO> Fields { get; set; }
        public virtual ICollection<PermissionDAO> Permissions { get; set; }
    }
}
