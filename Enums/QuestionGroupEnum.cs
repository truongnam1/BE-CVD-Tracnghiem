using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class QuestionGroupEnum
    {
        public static GenericEnum A1 = new GenericEnum (Id : 1, Code : "", Name : "");
        public static List<GenericEnum> QuestionGroupEnumList = new List<GenericEnum>
        {
            A1,
        };
    }
}
