using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_AppUserExportDTO : DataDTO
    {
        public long STT {get; set; }
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public long? ImageId { get; set; }
        public AppUser_ImageDTO Image { get; set; }
        public List<AppUser_ExamHistoryDTO> ExamHistories { get; set; }
        public AppUser_AppUserExportDTO() {}
        public AppUser_AppUserExportDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Password = AppUser.Password;
            this.RefreshToken = AppUser.RefreshToken;
            this.ImageId = AppUser.ImageId;
            this.Image = AppUser.Image == null ? null : new AppUser_ImageDTO(AppUser.Image);
            this.ExamHistories = AppUser.ExamHistories?.Select(x => new AppUser_ExamHistoryDTO(x)).ToList();
            this.Informations = AppUser.Informations;
            this.Warnings = AppUser.Warnings;
            this.Errors = AppUser.Errors;
        }
    }
}
