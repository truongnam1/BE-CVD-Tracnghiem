using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class PermissionActionMappingDAO
    {
        public long ActionId { get; set; }
        public long PermissionId { get; set; }

        public virtual ActionDAO Action { get; set; }
        public virtual PermissionDAO Permission { get; set; }
    }
}
