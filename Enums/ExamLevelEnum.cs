using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class ExamLevelEnum
    {
        public static GenericEnum Easy = new GenericEnum (Id : 1, Code : "Easy", Name : "Dễ");
        public static GenericEnum Normal = new GenericEnum(Id: 2, Code: "Normal", Name: "Trung bình");
        public static GenericEnum Hard = new GenericEnum(Id: 3, Code: "Hard", Name: "Khó");

        public static List<GenericEnum> ExamLevelEnumList = new List<GenericEnum>
        {
            Easy, Normal, Hard
        };
    }
}
