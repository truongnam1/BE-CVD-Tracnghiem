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
        public static GenericEnum Grade_5 = new GenericEnum(Id: 5, Code: "Grade_5", Name: "Lớp 5");
        public static GenericEnum Grade_6 = new GenericEnum(Id: 6, Code: "Grade_6", Name: "Lớp 6");
        public static GenericEnum Grade_7 = new GenericEnum(Id: 7, Code: "Grade_7", Name: "Lớp 7");
        public static GenericEnum Grade_8 = new GenericEnum(Id: 8, Code: "Grade_8", Name: "Lớp 8");
        public static GenericEnum Grade_9 = new GenericEnum(Id: 9, Code: "Grade_9", Name: "Lớp 9");
        public static GenericEnum Grade_10 = new GenericEnum(Id: 10, Code: "Grade_10", Name: "Lớp 10");
        public static GenericEnum Grade_11 = new GenericEnum(Id: 11, Code: "Grade_11", Name: "Lớp 11");
        public static GenericEnum Grade_12 = new GenericEnum(Id: 12, Code: "Grade_12", Name: "Lớp 12");


        public static List<GenericEnum> GradeEnumList = new List<GenericEnum>
        {
            Grade_1, Grade_2, Grade_3, Grade_4, Grade_5, Grade_6, Grade_7, Grade_8, Grade_9, Grade_10, Grade_11, Grade_12
        };
    }
}
