using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class GradeEnum
    {
        public static GenericEnum Grade_1 = new GenericEnum (Id : 1, Code : "Grade_1", Name : "Lớp 1");
        public static GenericEnum Grade_2 = new GenericEnum(Id: 2, Code: "Grade_2", Name: "Lớp 2");
        public static GenericEnum Grade_3 = new GenericEnum(Id: 3, Code: "Grade_3", Name: "Lớp 3");
        public static GenericEnum Grade_4 = new GenericEnum(Id: 4, Code: "Grade_4", Name: "Lớp 4");


        public static List<GenericEnum> GradeEnumList = new List<GenericEnum>
        {
            Grade_1, Grade_2, Grade_3, Grade_4
        };
    }
}
