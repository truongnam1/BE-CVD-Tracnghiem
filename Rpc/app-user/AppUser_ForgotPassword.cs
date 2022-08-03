using Tracnghiem.Entities;
using TrueSight.Common;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_ForgotPassword : DataDTO
    {
        public string Email { get; set; }
        public AppUser_ForgotPassword()
        { }
        public AppUser_ForgotPassword(AppUser AppUser)
        {
            this.Email = AppUser.Email;
            this.Errors = AppUser.Errors;
        }
    }
}
