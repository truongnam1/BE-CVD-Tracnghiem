using System;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        //public string DeviceName { get; set; }
        public string IdToken { get; set; }
    }
}
