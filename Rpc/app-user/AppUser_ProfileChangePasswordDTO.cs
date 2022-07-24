namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_ProfileChangePasswordDTO
    {
        public long Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
