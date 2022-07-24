using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class QuestionTypeEnum
    {
        


        public static GenericEnum ChooseOneAnswer = new GenericEnum (Id : 1, Code : "ChooseOneAnswer", Name : "Chọn một đáp án ");
        public static GenericEnum ChooseMultipleAnswer = new GenericEnum(Id: 2, Code: "ChooseMultipleAnswer", Name: "Chọn nhiều đáp án ");
        public static GenericEnum FillAnswer = new GenericEnum(Id: 3, Code: "FillAnswer", Name: "Điền đáp án ");

        public static List<GenericEnum> QuestionTypeEnumList = new List<GenericEnum>
        {
            ChooseOneAnswer, ChooseMultipleAnswer, FillAnswer
        };
    }
}
