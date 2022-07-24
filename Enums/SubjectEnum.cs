using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class SubjectEnum
    {
        public static GenericEnum A1 = new GenericEnum (Id : 1, Code : "", Name : "");
        public static List<GenericEnum> SubjectEnumList = new List<GenericEnum>
        {
            A1,
        };
    }
}
