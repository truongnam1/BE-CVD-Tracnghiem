using TrueSight.Common;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_RecoveryPasswordByOTPDTO : DataDTO
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
