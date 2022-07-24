using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class SubjectEnum
    {
        public static GenericEnum Economics = new GenericEnum(Id: 1, Code: "Economics", Name: "kinh tế học");
        public static GenericEnum Literature = new GenericEnum(Id: 2, Code: "Literature", Name: "ngữ văn");
        public static GenericEnum Geography = new GenericEnum(Id: 3, Code: "Geography", Name: "địa lý");
        public static GenericEnum History  = new GenericEnum(Id: 4, Code: "History ", Name: "lịch sử");
        public static GenericEnum CivicEducation = new GenericEnum(Id: 5, Code: "CivicEducation", Name: "Giáo dục công dân");
        public static GenericEnum Ethics = new GenericEnum(Id: 6, Code: "Ethics", Name: "Đạo đức");

        public static List<GenericEnum> SubjectEnumList = new List<GenericEnum>
        {
            Economics, Literature, Geography, History, CivicEducation, Ethics
        };
    }
}
