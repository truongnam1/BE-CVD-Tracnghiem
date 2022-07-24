using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Services.MExam
{
    public class ExamMessage
    {
        public enum Information
        {

        }

        public enum Warning
        {

        }

        public enum Error
        {
            IdNotExisted,
            ObjectUsed,
            CodeHasSpecialCharacter,
            CodeExisted,
            CodeEmpty,
            CodeOverLength,
            NameEmpty,
            NameOverLength,
            CreatorEmpty,
            CreatorNotExisted,
            ExamLevelEmpty,
            ExamLevelNotExisted,
            ExamStatusEmpty,
            ExamStatusNotExisted,
            GradeEmpty,
            GradeNotExisted,
            ImageNotExisted,
            StatusEmpty,
            StatusNotExisted,
            SubjectEmpty,
            SubjectNotExisted,
            ExamQuestionMapping_QuestionEmpty,
            ExamQuestionMapping_QuestionNotExisted,
        }
    }
}
