using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Services.MRole
{
    public interface IPermissionValidator : IServiceScoped
    {
        Task<bool> Create(Permission Permission);
        Task<bool> Update(Permission Permission);
        Task<bool> Delete(Permission Permission);
    }

    public class PermissionValidator : IPermissionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            CodeNotExisted,
            CodeEmpty,
            CodeOverLength,
            NameEmpty,
            NameOverLength,
            NameExisted,
            StatusNotExisted,
            PageNotExisted,
            FieldNotExisted,
            PermissionOperatorNotExisted,
            MenuNotInSiteOfRole,
            MenuExisted,
            MenuEmpty,
            ValueEmpty,
            PermissionContentEmpty,
            PermissionActionMappingEmpty,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public PermissionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Permission Permission)
        {
            PermissionFilter PermissionFilter = new PermissionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Permission.Id },
                Selects = PermissionSelect.Id
            };

            int count = await UOW.PermissionRepository.Count(PermissionFilter);
            if (count == 0)
                Permission.AddError(nameof(PermissionValidator), nameof(Permission.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateCode(Permission Permission)
        {
            if (string.IsNullOrWhiteSpace(Permission.Code))
                Permission.AddError(nameof(PermissionValidator), nameof(Permission.Code), ErrorCode.CodeEmpty);
            else
            {
                if (Permission.Code.Length > 255)
                    Permission.AddError(nameof(PermissionValidator), nameof(Permission.Code), ErrorCode.CodeOverLength);
                PermissionFilter PermissionFilter = new PermissionFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Permission.Id },
                    Code = new StringFilter { Equal = Permission.Code },
                    RoleId = new IdFilter { Equal = Permission.RoleId },
                    Selects = PermissionSelect.Code
                };

                int count = await UOW.PermissionRepository.Count(PermissionFilter);
                if (count != 0)
                    Permission.AddError(nameof(PermissionValidator), nameof(Permission.Code), ErrorCode.CodeExisted);
            }
            return Permission.IsValidated;
        }

        public async Task<bool> ValidateName(Permission Permission)
        {
            if (string.IsNullOrWhiteSpace(Permission.Name))
                Permission.AddError(nameof(PermissionValidator), nameof(Permission.Name), ErrorCode.NameEmpty);
            else
            {
                if (Permission.Name.Length > 255)
                    Permission.AddError(nameof(PermissionValidator), nameof(Permission.Name), ErrorCode.NameOverLength);
                PermissionFilter PermissionFilter = new PermissionFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Permission.Id },
                    Name = new StringFilter { Equal = Permission.Name },
                    RoleId = new IdFilter { Equal = Permission.RoleId },
                    Selects = PermissionSelect.Name
                };

                int count = await UOW.PermissionRepository.Count(PermissionFilter);
                if (count != 0)
                    Permission.AddError(nameof(PermissionValidator), nameof(Permission.Name), ErrorCode.NameExisted);
            }
            return Permission.IsValidated;

        }

        public async Task<bool> ValidateMenu(Permission Permission)
        {
            if (Permission.MenuId == 0)
            {
                Permission.AddError(nameof(PermissionValidator), nameof(Permission.Menu), ErrorCode.MenuEmpty);
            }
            else
            {
                var Role = await UOW.RoleRepository.Get(Permission.RoleId);
                var Menu = await UOW.MenuRepository.Get(Permission.MenuId);

                if (Menu?.SiteId != Role?.SiteId)
                    Permission.AddError(nameof(PermissionValidator), nameof(Permission.Menu), ErrorCode.MenuNotInSiteOfRole);

                int count = Role.Permissions.Where(x => x.MenuId == Permission.MenuId && x.Id != Permission.Id).Select(x => x.MenuId).Count();
                if (count != 0)
                    Permission.AddError(nameof(PermissionValidator), nameof(Permission.Menu), ErrorCode.MenuExisted);
            }

            return Permission.IsValidated;
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
                Permission.AddError(nameof(PermissionValidator), "Status", ErrorCode.StatusNotExisted);
            return count != 0;
        }

        public async Task<bool> Create(Permission Permission)
        {
            await ValidateMenu(Permission);
            await ValidateCode(Permission);
            await ValidateName(Permission);
            await ValidateStatus(Permission);
            await ValidatePermissionContent(Permission);
            return Permission.IsValidated;
        }

        public async Task<bool> Update(Permission Permission)
        {
            if (await ValidateId(Permission))
            {
                await ValidateMenu(Permission);
                await ValidateCode(Permission);
                await ValidateName(Permission);
                await ValidateStatus(Permission);
                await ValidatePermissionContent(Permission);
            }
            return Permission.IsValidated;
        }

        public async Task<bool> Delete(Permission Permission)
        {
            if (await ValidateId(Permission))
            {
            }
            return Permission.IsValidated;
        }

        public async Task<bool> ValidatePermissionContent(Permission Permission)
        {
            if (Permission.PermissionActionMappings == null || Permission.PermissionActionMappings.Count == 0)
            {
                Permission.AddError(nameof(PermissionValidator), nameof(PermissionActionMapping), ErrorCode.PermissionActionMappingEmpty);
            }
            else
            {
                List<Field> Fields = await UOW.FieldRepository.List(new FieldFilter
                {
                    Selects = FieldSelect.ALL,
                    Skip = 0,
                    Take = int.MaxValue
                });
                List<PermissionOperator> PermissionOperators = await UOW.PermissionOperatorRepository.List(new PermissionOperatorFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                });

                if (Permission.PermissionContents != null)
                {
                    foreach (PermissionContent PermissionContent in Permission.PermissionContents)
                    {
                        Field Field = Fields.Where(f => f.Id == PermissionContent.FieldId).FirstOrDefault();
                        if (Field == null)
                        {
                            PermissionContent.AddError(nameof(PermissionValidator), nameof(PermissionContent.Field), ErrorCode.FieldNotExisted);
                        }
                        else
                        {
                            PermissionOperator PermissionOperator = PermissionOperators
                                .Where(po => po.FieldTypeId == Field.FieldTypeId && po.Id == PermissionContent.PermissionOperatorId).FirstOrDefault();
                            if (PermissionOperator == null)
                                PermissionContent.AddError(nameof(PermissionValidator), nameof(PermissionContent.PermissionOperator), ErrorCode.PermissionOperatorNotExisted);
                        }
                        if (PermissionContent.Value == null)
                            PermissionContent.AddError(nameof(PermissionValidator), nameof(PermissionContent.Value), ErrorCode.ValueEmpty);
                    }
                }
            }
            return Permission.IsValidated;
        }
    }
}
