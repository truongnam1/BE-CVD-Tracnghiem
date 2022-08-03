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
            ObjectNotExisted,
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

            AppUserInUsed,
            UsernameExisted,
            UsernameHasSpecialCharacter,
            EmailExisted,
            EmailEmpty,
            EmailOverLength,
            EmailInvalid,
            PhoneEmpty,
            PhoneInvalid,
            AddressOverLength,
            RefreshTokenInvalid,
            RefreshTokenExpired,
            StatusNotExisted,
            SexEmpty,
            OrganizationNotExisted,
            OrganizationEmpty,
            UsernameNotExisted,
            PasswordNotMatch,
            ProvinceNotExisted,
            ProvinceEmpty,
            EmailNotExisted,
            OtpCodeInvalid,
            OtpCodeEmpty,
            OtpExpired,
            PositionIdNotExisted,
            PositionEmpty,
            LDAPAuthenticationFailed,
            BirthdayInvalid,
            ProjectNotExisted,
            SiteCodeEmpty,
            IdTokenInvalid
        }
    }
}
