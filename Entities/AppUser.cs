using TrueSight.Common;
using System;
using System.Collections.Generic;
using Tracnghiem.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tracnghiem.Helpers;
using TrueSight.PER.Entities;

namespace Tracnghiem.Entities
{
    public class AppUser : DataEntity,  IEquatable<AppUser>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpired { get; set; }
        public string RefreshToken { get; set; }
        public long? RoleId { get; set; }
        public long? ImageId { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public Image Image { get; set; }
        public Role Role { get; set; }
        public RefreshTokenObject RefreshTokenObject { get; set; }

        public List<ExamHistory> ExamHistories { get; set; }
        
        public bool Equals(AppUser other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Username != other.Username) return false;
            if (this.DisplayName != other.DisplayName) return false;
            if (this.Password != other.Password) return false;
            if (this.RefreshToken != other.RefreshToken) return false;
            if (this.RoleId != other.RoleId) return false;
            if (this.ImageId != other.ImageId) return false;
            if (this.ExamHistories?.Count != other.ExamHistories?.Count) return false;
            else if (this.ExamHistories != null && other.ExamHistories != null)
            {
                for (int i = 0; i < ExamHistories.Count; i++)
                {
                    ExamHistory ExamHistory = ExamHistories[i];
                    ExamHistory otherExamHistory = other.ExamHistories[i];
                    if (ExamHistory == null && otherExamHistory != null)
                        return false;
                    if (ExamHistory != null && otherExamHistory == null)
                        return false;
                    if (ExamHistory.Equals(otherExamHistory) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AppUserFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter RefreshToken { get; set; }
        public IdFilter RoleId { get; set; }
        public IdFilter ImageId { get; set; }
        public List<AppUserFilter> OrFilter { get; set; }
        public AppUserOrder OrderBy {get; set;}
        public AppUserSelect Selects {get; set;}
        public AppUserSearch SearchBy {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserOrder
    {
        Id = 0,
        Username = 1,
        DisplayName = 2,
        Password = 3,
        RefreshToken = 4,
        Role = 5,
        Image = 6,
        Email = 7,
    }

    [Flags]
    public enum AppUserSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Username = E._1,
        DisplayName = E._2,
        Password = E._3,
        RefreshToken = E._4,
        Role = E._5,
        Image = E._6,
        Email = E._7,
    }

    [Flags]
    public enum AppUserSearch:long
    {
        ALL = E.ALL,
        Username = E._1,
        DisplayName = E._2,
        Password = E._3,
        RefreshToken = E._4,
        Email = E._5,
    }
}
