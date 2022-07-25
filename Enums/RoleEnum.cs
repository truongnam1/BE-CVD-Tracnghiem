using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class RoleEnum
    {
        public static GenericEnum UserRole = new GenericEnum (Id : 1, Code : "Tracnghiem_User", Name : "Tracnghiem_User");
        public static List<GenericEnum> RoleEnumList = new List<GenericEnum>
        {
            UserRole
        };
    }
}
