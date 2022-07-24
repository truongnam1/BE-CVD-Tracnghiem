using System;
using System.Collections.Generic;

namespace Tracnghiem.Models
{
    public partial class AggregatedCounterDAO
    {
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
