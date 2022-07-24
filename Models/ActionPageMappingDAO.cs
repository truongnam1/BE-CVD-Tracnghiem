using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class ActionPageMappingDAO
    {
        public long ActionId { get; set; }
        public long PageId { get; set; }

        public virtual ActionDAO Action { get; set; }
        public virtual PageDAO Page { get; set; }
    }
}
