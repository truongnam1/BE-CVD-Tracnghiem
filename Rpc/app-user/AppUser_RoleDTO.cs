using Tracnghiem.Entities;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Rpc.app_user
{
    public class AppUser_RoleDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }


        public AppUser_RoleDTO() { }
        public AppUser_RoleDTO(Role Role)
        {

            this.Id = Role.Id;

            this.Code = Role.Code;

            this.Name = Role.Name;

            this.StatusId = Role.StatusId;

        }
    }

    public class AppUser_RoleFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public RoleOrder OrderBy { get; set; }
    }
}