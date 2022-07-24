using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class ExamStatusEnum
    {
         
 
        public static GenericEnum Draft = new GenericEnum (Id : 1, Code : "Draft", Name : "Nháp");
        public static GenericEnum Publish = new GenericEnum(Id: 2, Code: "Publish", Name: "Đã xuất bản");

        public static List<GenericEnum> ExamStatusEnumList = new List<GenericEnum>
        {
            Draft, Publish
        };
    }
}
