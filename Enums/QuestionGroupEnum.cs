using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Enums
{
    public class QuestionGroupEnum
    {
        
    
    
   
    
   
        public static GenericEnum KnowledgeQuestion = new GenericEnum (Id : 1, Code : "KnowledgeQuestion", Name : "câu hỏi biết");
        public static GenericEnum UnderstandQuestion = new GenericEnum(Id: 2, Code: "UnderstandQuestion", Name: "câu hỏi hiểu");
        public static GenericEnum ApplicationQuestion = new GenericEnum(Id: 3, Code: "ApplicationQuestion", Name: "câu hỏi vân dụng");
        public static GenericEnum AnalysisQuestion = new GenericEnum(Id: 4, Code: "AnalysisQuestion", Name: "câu hỏi phân tích");
        public static GenericEnum SynthesisQuestion = new GenericEnum(Id: 5, Code: "SynthesisQuestion", Name: "câu hỏi tổng hợp");
        public static GenericEnum AssessmentQuestion = new GenericEnum(Id: 6, Code: "AssessmentQuestion", Name: "câu hỏi đánh giá ");

        public static List<GenericEnum> QuestionGroupEnumList = new List<GenericEnum>
        {
            KnowledgeQuestion, UnderstandQuestion, ApplicationQuestion, AnalysisQuestion, SynthesisQuestion, AssessmentQuestion
        };
    }
}
