using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracnghiem.Services.MAppUser
{
    public class AppUserMessage
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
            UsernameEmpty,
            UsernameOverLength,
            DisplayNameEmpty,
            DisplayNameOverLength,
            PasswordEmpty,
            PasswordOverLength,
            RefreshTokenEmpty,
            RefreshTokenOverLength,
            ImageNotExisted,
            RoleEmpty,
            RoleNotExisted,
            ExamHistory_ExamedAtEmpty,
            ExamHistory_ExamedAtInvalid,
            ExamHistory_ExamEmpty,
            ExamHistory_ExamNotExisted,
        }
    }
}
