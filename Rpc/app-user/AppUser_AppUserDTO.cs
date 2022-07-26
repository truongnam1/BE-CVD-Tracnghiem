using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public long? ImageId { get; set; }
        public string Token { get; set; }
        public AppUser_ImageDTO Image { get; set; }
        public List<AppUser_ExamHistoryDTO> ExamHistories { get; set; }
        public AppUser_AppUserDTO() {}
        public AppUser_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Email = AppUser.Email;
            this.Password = AppUser.Password;
            this.RefreshToken = AppUser.RefreshToken;
            this.ImageId = AppUser.ImageId;
            this.Token = AppUser.Token;
            this.Image = AppUser.Image == null ? null : new AppUser_ImageDTO(AppUser.Image);
            this.ExamHistories = AppUser.ExamHistories?.Select(x => new AppUser_ExamHistoryDTO(x)).ToList();
            this.Informations = AppUser.Informations;
            this.Warnings = AppUser.Warnings;
            this.Errors = AppUser.Errors;
        }
    }

    public class AppUser_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter RefreshToken { get; set; }
        public IdFilter ImageId { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
