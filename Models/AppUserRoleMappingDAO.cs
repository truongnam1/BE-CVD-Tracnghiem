using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class AppUserRoleMappingDAO
    {
        public long AppUserId { get; set; }
        public long RoleId { get; set; }

        public virtual Role1DAO Role { get; set; }
    }
}
