using System;

namespace Tracnghiem.Helpers
{
    public class RefreshTokenObject
    {
        public long AppUserId { get; set; }
        public string AppUserUsername { get; set; }
        public string DeviceName { get; set; }
        public DateTime Issues { get; set; }
        public DateTime Expires { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsExpired()
        {
            return Expires.CompareTo(StaticParams.DateTimeNow) < 0;
        }

    }
}
