using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.public_exam
{
    public class PublicExam_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public long? RoleId { get; set; }
        public long? ImageId { get; set; }
        public PublicExam_AppUserDTO() {}
        public PublicExam_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.DisplayName = AppUser.DisplayName;
            this.Password = AppUser.Password;
            this.RefreshToken = AppUser.RefreshToken;
            this.RoleId = AppUser.RoleId;
            this.ImageId = AppUser.ImageId;
            this.Informations = AppUser.Informations;
            this.Warnings = AppUser.Warnings;
            this.Errors = AppUser.Errors;
        }
    }

    public class PublicExam_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter RefreshToken { get; set; }
        public IdFilter RoleId { get; set; }
        public IdFilter ImageId { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}