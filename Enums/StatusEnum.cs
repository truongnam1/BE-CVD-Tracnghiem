using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class StatusEnum
    {
        public static GenericEnum INACTIVE = new GenericEnum (Id : 0, Code : "INACTIVE", Name : "Ngừng hoạt động", Color : "#525252");
        public static GenericEnum ACTIVE = new GenericEnum (Id : 1, Code : "ACTIVE", Name : "Hoạt động", Color: "#24a148");
        public static List<GenericEnum> StatusEnumList = new List<GenericEnum>
        {
            INACTIVE, ACTIVE,
        };
    }
}
