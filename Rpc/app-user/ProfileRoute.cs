using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using Tracnghiem.Entities;
using Tracnghiem.Services.MAppUser;
using System.ComponentModel;
using Tracnghiem.Services.MImage;
using Tracnghiem.Services.MRole;
using Tracnghiem.Services.MExamHistory;
using Tracnghiem.Services.MExam;

namespace Tracnghiem.Rpc.app_user
{
    [DisplayName("Profile")]
    public class ProfileRoute : Root
    {
        private const string Default = Rpc + Module;
        public const string Login = Default + "/account/login";
        public const string Logged = Default + "/account/logged";
        public const string GetForWeb = Default + "/profile-web/get";
        public const string Get = Default + "/profile/get";
        public const string Update = Default + "/profile/update";
        public const string SaveImage = Default + "/profile/save-image";
        public const string ChangePassword = Default + "/profile/change-password";
        public const string RecoveryPasswordByOTP = Default + "/profile/recovery-password-by-otp";
        public const string ForgotPassword = Default + "/profile/forgot-password";
        public const string VerifyOtpCode = Default + "/profile/verify-otp-code";
        public const string RecoveryPassword = Default + "/profile/recovery-password";
        public const string RefreshToken = Default + "/profile/refresh-token";
        public const string CreateSession = Default + "/profile/create-session";

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Đăng nhập", new List<string> {
                    Logged,
                    GetForWeb, Get, Update, SaveImage, SaveImage,
                }
            },
            { "Đổi mật khẩu", new List<string> {
                    ChangePassword
                }
            }
        };
    }
}
