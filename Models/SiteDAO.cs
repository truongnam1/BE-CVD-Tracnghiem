using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class SiteDAO
    {
        public SiteDAO()
        {
            Menus = new HashSet<MenuDAO>();
            Roles = new HashSet<RoleDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsDisplay { get; set; }

        public virtual ICollection<MenuDAO> Menus { get; set; }
        public virtual ICollection<RoleDAO> Roles { get; set; }
    }
}
