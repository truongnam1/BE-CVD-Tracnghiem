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
        public static GenericEnum Public = new GenericEnum(Id: 2, Code: "Public", Name: "Công khai");
        public static GenericEnum Private = new GenericEnum(Id: 3, Code: "Private", Name: "Riêng tư");


        public static List<GenericEnum> ExamStatusEnumList = new List<GenericEnum>
        {
            Draft, Public, Private
        };
    }
}
