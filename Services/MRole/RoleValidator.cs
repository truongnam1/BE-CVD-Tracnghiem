using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Tracnghiem.Helpers;
using Tracnghiem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Services.MRole
{
    public interface IRoleValidator : IServiceScoped
    {
        Task<bool> Create(Role Role);
        Task<bool> Update(Role Role);
        Task<bool> AssignAppUser(Role Role);
        Task<bool> Delete(Role Role);
        Task<bool> BulkDelete(List<Role> Roles);
        Task<bool> Import(List<Role> Roles);
    }

    public class RoleValidator : IRoleValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            CodeNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeOverLength,
            NameEmpty,
            NameOverLength,
            NameExisted,
            StatusNotExisted,
            PageNotExisted,
            FieldNotExisted,
            AppUserNotExisted,
            StartAtInvalid,
            EndAtInvalid,
            EndAtWrong,
            StatusInvalid,
            RoleHasAsignToAppUser,
            RoleInUsed,
            UpdateSiteInvalid,
            SiteEmpty,
            SiteNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public RoleValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Role Role)
        {
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Role.Id },
                Selects = RoleSelect.Id
            };

            int count = await UOW.RoleRepository.Count(RoleFilter);
            if (count == 0)
                Role.AddError(nameof(RoleValidator), nameof(Role.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateCode(Role Role)
        {
            if (string.IsNullOrWhiteSpace(Role.Code))
                Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeEmpty);
            else
            {
                var Code = Role.Code;
                if (Role.Code.Contains(" ") || !Code.ChangeToEnglishChar().Equals(Role.Code))
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeHasSpecialCharacter);
                }
                else if (Role.Code.Length > 255)
                    Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeOverLength);
                else
                {
                    RoleFilter RoleFilter = new RoleFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Role.Id },
                        Code = new StringFilter { Equal = Role.Code },
                        Selects = RoleSelect.Code
                    };
                    int count = await UOW.RoleRepository.Count(RoleFilter);
                    if (count != 0)
                        Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeExisted);
                }

            }
            return Role.IsValidated;
        }

        public async Task<bool> ValidateName(Role Role)
        {
            if (string.IsNullOrWhiteSpace(Role.Name))
                Role.AddError(nameof(RoleValidator), nameof(Role.Name), ErrorCode.NameEmpty);
            else
            {
                if (Role.Name.Length > 255)
                    Role.AddError(nameof(RoleValidator), nameof(Role.Name), ErrorCode.NameOverLength);
                RoleFilter RoleFilter = new RoleFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Role.Id },
                    Name = new StringFilter { Equal = Role.Name },
                    Selects = RoleSelect.Name
                };

                int count = await UOW.RoleRepository.Count(RoleFilter);
                if (count != 0)
                    Role.AddError(nameof(RoleValidator), nameof(Role.Name), ErrorCode.NameExisted);
            }
            return Role.IsValidated;

        }

        public async Task<bool> ValidateStatus(Role Role)
        {
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Role.StatusId },
                Selects = StatusSelect.Id
            };
            int count = await UOW.StatusRepository.Count(StatusFilter);
            if (count == 0)
                Role.AddError(nameof(RoleValidator), "Status", ErrorCode.StatusNotExisted);

            if (Role.StatusId == StatusEnum.ACTIVE.Id)
            {
                if (Role.StartAt.HasValue && Role.StartAt.Value.Date > StaticParams.DateTimeNow.Date)
                {
                    Role.AddError(nameof(RoleValidator), "Status", ErrorCode.StatusInvalid);
                }
            }

            return Role.IsValidated;
        }

        private async Task<bool> ValidateStartDateAndEndDate(Role Role)
        {
            if (Role.EndAt.HasValue)
            {
                if (Role.EndAt.Value.Date < StaticParams.DateTimeNow.Date)
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.EndAt), ErrorCode.EndAtWrong);
                }
                else if (Role.EndAt.Value < Role.StartAt)
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.EndAt), ErrorCode.EndAtWrong);
                }
            }

            return Role.IsValidated;
        }
        public async Task<bool> ValidateStatus(Permission Permission)
        {
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Permission.StatusId },
                Selects = StatusSelect.Id
            };
            int count = await UOW.StatusRepository.Count(StatusFilter);
            if (count == 0)
                Permission.AddError(nameof(RoleValidator), "Status", ErrorCode.StatusNotExisted);

            if (Permission.EndAt != null)
            {
                if (Permission.EndAt <= DateTime.Now.AddHours(0 - CurrentContext.TimeZone).Date)
                    Permission.AddError(nameof(RoleValidator), "Status", ErrorCode.EndAtInvalid);
                if (Permission.StartAt.HasValue && Permission.StartAt >= Permission.EndAt)
                    Permission.AddError(nameof(RoleValidator), "Status", ErrorCode.StartAtInvalid);
            }

            if (Permission.StatusId == StatusEnum.ACTIVE.Id)
            {
                if (Permission.StartAt.HasValue && Permission.StartAt > DateTime.Now.AddHours(0 - CurrentContext.TimeZone).Date)
                {
                    Permission.AddError(nameof(RoleValidator), "Status", ErrorCode.StatusInvalid);
                }
                if (Permission.EndAt.HasValue && Permission.EndAt <= DateTime.Now.AddHours(0 - CurrentContext.TimeZone).Date)
                {
                    Permission.AddError(nameof(RoleValidator), "Status", ErrorCode.StatusInvalid);
                }
            }

            return Permission.IsValidated;
        }

        public async Task<bool> ValidatePermission(Role Role)
        {
            foreach (var Permission in Role.Permissions)
            {
                PermissionFilter PermissionFilter = new PermissionFilter
                {
                    Skip = 0,
                    Take = 10,
                    Code = new StringFilter { Equal = Permission.Code },
                    Selects = PermissionSelect.Code
                };

                int count = await UOW.PermissionRepository.Count(PermissionFilter);
                if (count == 0)
                {
                    Permission.AddError(nameof(RoleValidator), nameof(Permission.Code), ErrorCode.CodeNotExisted);
                }

                await ValidateMenu(Permission.Menu);
                await ValidateStatus(Permission);
            }
            return Role.Permissions.Any(s => !s.IsValidated) ? false : true;
        }

        public async Task<bool> ValidateMenu(Menu Menu)
        {
            MenuFilter MenuFilter = new MenuFilter
            {
                Skip = 0,
                Take = 10,
                Code = new StringFilter { Equal = Menu.Code },
                Selects = MenuSelect.Code
            };

            var MenuInDB = (await UOW.MenuRepository.List(MenuFilter)).FirstOrDefault();
            if (MenuInDB == null)
            {
                Menu.AddError(nameof(RoleValidator), nameof(Menu.Code), ErrorCode.CodeNotExisted);
            }
            else
            {
                foreach (var Field in Menu.Fields)
                {
                    if (!MenuInDB.Fields.Select(f => f.Name).Contains(Field.Name))
                    {
                        Field.AddError(nameof(RoleValidator), nameof(Field.Name), ErrorCode.FieldNotExisted);
                        return false;
                    }
                }

                //foreach (var Page in Menu.Actions)
                //{
                //    if (!MenuInDB.Actions.Select(p => p.Path).Contains(Page.Path))
                //    {
                //        Page.AddError(nameof(RoleValidator), nameof(Page.Path), ErrorCode.PageNotExisted);
                //        return false;
                //    }
                //}
            }
            return Menu.IsValidated;
        }

        private async Task<bool> ValidateAssignAppUser(Role Role)
        {
            List<long> ids = Role.AppUserRoleMappings.Select(a => a.AppUserId).ToList();
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = ids },
                Selects = AppUserSelect.Id,
                OrderBy = AppUserOrder.Id,
            };

            var listInDB = await UOW.AppUserRepository.List(AppUserFilter);
            var listExcept = Role.AppUserRoleMappings.Select(a => a.AppUserId).Except(listInDB.Select(a => a.Id));
            if (listExcept.Any())
            {
                foreach (var AppUserID in listExcept)
                {
                    Role.AddError(nameof(RoleValidator), AppUserID.ToString(), ErrorCode.AppUserNotExisted);
                }
            }

            return Role.IsValidated;
        }

        private async Task<bool> ValidateSite(Role Role)
        {
            if (Role.SiteId == null)
            {
                Role.AddError(nameof(RoleValidator), nameof(Role.Site), ErrorCode.SiteEmpty);
            }
            else
            {
                long SiteCounter = await UOW.SiteRepository.Count(new SiteFilter
                {
                    Id = new IdFilter { Equal = Role.SiteId },
                });
                if (SiteCounter == 0)
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.Site), ErrorCode.SiteNotExisted);
                }    
                var OldRole = await UOW.RoleRepository.Get(Role.Id);

                if (OldRole != null && (OldRole.Permissions != null && OldRole.Permissions.Count > 0) && (OldRole.SiteId != Role.SiteId))
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.Site), ErrorCode.UpdateSiteInvalid);
                }
            }

            return Role.IsValidated;
        }

        public async Task<bool> Create(Role Role)
        {
            await ValidateCode(Role);
            await ValidateSite(Role);
            await ValidateName(Role);
            await ValidateStatus(Role);
            await ValidateStartDateAndEndDate(Role);
            return Role.IsValidated;
        }

        public async Task<bool> Update(Role Role)
        {
            if (await ValidateId(Role))
            {
                await ValidateSite(Role);
                await ValidateCode(Role);
                await ValidateName(Role);
                await ValidateStatus(Role);
                await ValidateStartDateAndEndDate(Role);
            }
            return Role.IsValidated;
        }

        public async Task<bool> AssignAppUser(Role Role)
        {
            if (await ValidateId(Role))
            {
                await ValidateAssignAppUser(Role);
            }
            return Role.IsValidated;
        }

        public async Task<bool> Delete(Role Role)
        {
            if (await ValidateId(Role))
            {
                var oldData = await UOW.RoleRepository.Get(Role.Id);
                if (oldData.AppUserRoleMappings != null && oldData.AppUserRoleMappings.Count > 0)
                    Role.AddError(nameof(RoleValidator), nameof(Role.AppUserRoleMappings), ErrorCode.RoleHasAsignToAppUser);
                if(oldData.Used)
                    Role.AddError(nameof(RoleValidator), nameof(Role.Used), ErrorCode.RoleInUsed);
            }
            return Role.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Role> Roles)
        {
            RoleFilter RoleFilter = new RoleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = Roles.Select(a => a.Id).ToList() },
                Selects = RoleSelect.Id
            };

            var listInDB = await UOW.RoleRepository.List(RoleFilter);
            var listExcept = Roles.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var Role in listExcept)
            {
                Role.AddError(nameof(RoleValidator), nameof(Role.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }

        public async Task<bool> Import(List<Role> Roles)
        {
            var listCodeInDB = (await UOW.RoleRepository.List(new RoleFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = RoleSelect.Code
            })).Select(e => e.Code);

            foreach (var Role in Roles)
            {
                if (listCodeInDB.Contains(Role.Code))
                {
                    Role.AddError(nameof(RoleValidator), nameof(Role.Code), ErrorCode.CodeExisted);
                    return false;
                }
                await ValidatePermission(Role);
            }
            return Roles.Any(s => !s.IsValidated) ? false : true;
        }
    }
}
