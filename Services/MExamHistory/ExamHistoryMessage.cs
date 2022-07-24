using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Services.MExamHistory
{
    public class ExamHistoryMessage
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
            ExamedAtInvalid,
            ExamedAtEmpty,
            AppUserEmpty,
            AppUserNotExisted,
            ExamEmpty,
            ExamNotExisted,
        }
    }
}
