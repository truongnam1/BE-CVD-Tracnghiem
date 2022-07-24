using TrueSight.Common;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_RefreshTokenResultDTO : DataDTO
    {
        public string RefreshToken { get; set; }
        public string Token { get; set; }
    }
}