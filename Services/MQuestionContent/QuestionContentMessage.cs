using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Services.MQuestionContent
{
    public class QuestionContentMessage
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
            AnswerContentEmpty,
            AnswerContentOverLength,
            QuestionEmpty,
            QuestionNotExisted,
        }
    }
}
