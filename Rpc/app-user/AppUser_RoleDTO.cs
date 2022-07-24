using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_RoleDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public AppUser_RoleDTO() {}
        public AppUser_RoleDTO(Role Role)
        {
            this.Id = Role.Id;
            this.Code = Role.Code;
            this.Name = Role.Name;
            this.Informations = Role.Informations;
            this.Warnings = Role.Warnings;
            this.Errors = Role.Errors;
        }
    }

    public class AppUser_RoleFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public RoleOrder OrderBy { get; set; }
    }
}