using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class PermissionDAO
    {
        public PermissionDAO()
        {
            PermissionActionMappings = new HashSet<PermissionActionMappingDAO>();
            PermissionContents = new HashSet<PermissionContentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        public long StatusId { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual Role1DAO Role { get; set; }
        public virtual ICollection<PermissionActionMappingDAO> PermissionActionMappings { get; set; }
        public virtual ICollection<PermissionContentDAO> PermissionContents { get; set; }
    }
}
