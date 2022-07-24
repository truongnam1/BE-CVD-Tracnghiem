using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Services.MQuestion
{
    public class QuestionMessage
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
            ContentEmpty,
            ContentOverLength,
            GradeEmpty,
            GradeNotExisted,
            QuestionGroupEmpty,
            QuestionGroupNotExisted,
            QuestionTypeEmpty,
            QuestionTypeNotExisted,
            StatusEmpty,
            StatusNotExisted,
            SubjectEmpty,
            SubjectNotExisted,
            QuestionContent_AnswerContentEmpty,
            QuestionContent_AnswerContentOverLength,
        }
    }
}
