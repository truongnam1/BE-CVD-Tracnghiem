using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ActionDAO
    {
        public ActionDAO()
        {
            ActionPageMappings = new HashSet<ActionPageMappingDAO>();
            PermissionActionMappings = new HashSet<PermissionActionMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual ICollection<ActionPageMappingDAO> ActionPageMappings { get; set; }
        public virtual ICollection<PermissionActionMappingDAO> PermissionActionMappings { get; set; }
    }
}
