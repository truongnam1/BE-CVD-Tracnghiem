using Tracnghiem.Entities;
using Tracnghiem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using Tracnghiem.Enums;
using TrueSight.PER.Entities;
using Action = TrueSight.PER.Entities.Action;
using Microsoft.Extensions.Configuration;

namespace Tracnghiem.Repositories
{
    public interface IPermissionRepository
    {
        Task<int> Count(PermissionFilter PermissionFilter);
        Task<List<Permission>> List(PermissionFilter PermissionFilter);
        Task<List<Role>> ListRole(Permission Permission);
        Task<Permission> Get(long Id);
        Task<bool> Create(Permission Permission);
        Task<bool> Update(Permission Permission);
        Task<List<Role>> Delete(Permission Permission);
        Task<bool> BulkMerge(List<Permission> Permissions);
        Task<bool> BulkDelete(List<Permission> Permissions);
        Task<List<long>> ListAppUser(string Path);
        Task<List<string>> ListPath(long AppUserId);
        Task<List<CurrentPermission>> ListByUserAndPath(long AppUserId, string Path);
    }
    public class PermissionRepository : CacheRepository, IPermissionRepository
    {
        private DataContext DataContext;
        public PermissionRepository(DataContext DataContext, IRedisStore RedisStore, IConfiguration Configuration)
           : base(DataContext, RedisStore, Configuration)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PermissionDAO> DynamicFilter(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.RoleId, filter.RoleId);
            query = query.Where(q => q.MenuId, filter.MenuId);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PermissionDAO> OrFilter(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PermissionDAO> initQuery = query.Where(q => false);
            foreach (PermissionFilter PermissionFilter in filter.OrFilter)
            {
                IQueryable<PermissionDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, PermissionFilter.Id);
                queryable = queryable.Where(q => q.Code, PermissionFilter.Code);
                queryable = queryable.Where(q => q.Name, PermissionFilter.Name);
                queryable = queryable.Where(q => q.RoleId, PermissionFilter.RoleId);
                queryable = queryable.Where(q => q.MenuId, PermissionFilter.MenuId);
                queryable = queryable.Where(q => q.StatusId, PermissionFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<PermissionDAO> DynamicOrder(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PermissionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PermissionOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PermissionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PermissionOrder.Role:
                            query = query.OrderBy(q => q.RoleId);
                            break;
                        case PermissionOrder.Menu:
                            query = query.OrderBy(q => q.MenuId);
                            break;
                        case PermissionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PermissionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PermissionOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PermissionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PermissionOrder.Role:
                            query = query.OrderByDescending(q => q.RoleId);
                            break;
                        case PermissionOrder.Menu:
                            query = query.OrderByDescending(q => q.MenuId);
                            break;
                        case PermissionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Permission>> DynamicSelect(IQueryable<PermissionDAO> query, PermissionFilter filter)
        {
            List<Permission> Permissions = await query.Select(q => new Permission()
            {
                Id = filter.Selects.Contains(PermissionSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PermissionSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PermissionSelect.Name) ? q.Name : default(string),
                RoleId = filter.Selects.Contains(PermissionSelect.Role) ? q.RoleId : default(long),
                MenuId = filter.Selects.Contains(PermissionSelect.Menu) ? q.MenuId : default(long),
                //IsDeleted = filter.Selects.Contains(PermissionSelect.IsDeleted) ? q.IsDeleted : default(bool),
                StatusId = filter.Selects.Contains(PermissionSelect.Status) ? q.StatusId : default(long),
                Menu = filter.Selects.Contains(PermissionSelect.Menu) && q.Menu != null ? new Menu
                {
                    Id = q.Menu.Id,
                    Code = q.Menu.Code,
                    Name = q.Menu.Name,
                    IsDeleted = q.Menu.IsDeleted,
                    SiteId = q.Menu.SiteId,
                    //Site = q.Menu.Site == null ? null : new Site
                    //{
                    //    Id = q.Menu.Site.Id,
                    //    Code = q.Menu.Site.Code,
                    //    Name = q.Menu.Site.Name,
                    //    Description = q.Menu.Site.Description,
                    //    IsDisplay = q.Menu.Site.IsDisplay,
                    //    RowId = q.Menu.Site.RowId,
                    //} 
                } : null,
                Role = filter.Selects.Contains(PermissionSelect.Role) && q.Role != null ? new Role
                {
                    Id = q.Role.Id,
                    Code = q.Role.Code,
                    Name = q.Role.Name,
                    StatusId = q.StatusId
                } : null,
            }).ToListAsync();
            if (filter.Selects.Contains(PermissionSelect.PermissionContent))
            {
                List<long> Ids = Permissions.Select(x => x.Id).ToList();
                List<PermissionContent> PermissionContents = await DataContext.PermissionContent
                    .Where(x => Ids.Contains(x.PermissionId))
                    .Select(x => new PermissionContent
                    {
                        PermissionId = x.PermissionId,
                        FieldId = x.FieldId,
                        PermissionOperatorId = x.PermissionOperatorId,
                        Value = x.Value,
                        Id = x.Id,
                        //IsDeleted = x.IsDeleted,
                    }).ToListAsync();
                List<PermissionActionMapping> PermissionActionMappings = await DataContext.PermissionActionMapping
                    .Where(x => Ids.Contains(x.PermissionId))
                    .Select(x => new PermissionActionMapping
                    {
                        ActionId = x.ActionId,
                        PermissionId = x.PermissionId,
                    }).ToListAsync();
                foreach (Permission Permission in Permissions)
                {
                    Permission.PermissionContents = PermissionContents
                        .Where(x => x.PermissionId == Permission.Id)
                        .ToList();
                    Permission.PermissionActionMappings = PermissionActionMappings
                        .Where(x => x.PermissionId == Permission.Id)
                        .ToList();
                }
            }

            return Permissions;
        }

        public async Task<int> Count(PermissionFilter filter)
        {
            IQueryable<PermissionDAO> Permissions = DataContext.Permission;
            Permissions = DynamicFilter(Permissions, filter);
            return await Permissions.CountAsync();
        }

        public async Task<List<Permission>> List(PermissionFilter filter)
        {
            if (filter == null) return new List<Permission>();
            IQueryable<PermissionDAO> PermissionDAOs = DataContext.Permission.AsNoTracking();
            PermissionDAOs = DynamicFilter(PermissionDAOs, filter);
            PermissionDAOs = DynamicOrder(PermissionDAOs, filter);
            List<Permission> Permissions = await DynamicSelect(PermissionDAOs, filter);
            return Permissions;
        }
        public async Task<List<Role>> ListRole(Permission Permission)
        {
            List<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking()
                .Where(x => x.Id == Permission.RoleId)
                .ToList();
            List<PermissionDAO> PermissionDAOs = DataContext.Permission.AsNoTracking().ToList();
            List<PermissionContentDAO> PermissionContentDAOs = DataContext.PermissionContent.AsNoTracking().ToList();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = DataContext.PermissionActionMapping.AsNoTracking().ToList();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = DataContext.AppUserRoleMapping.AsNoTracking().ToList();
            List<Role> Roles = RoleDAOs.Select(x => new Role
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    StatusId = x.StatusId,
                    IsDeleted = x.IsDeleted,
                    Permissions = PermissionDAOs.Where(p => p.RoleId == x.Id).Select(pe => new Permission
                    {
                        Id = pe.Id,
                        Code = pe.Code,
                        Name = pe.Name,
                        RoleId = pe.RoleId,
                        MenuId = pe.MenuId,
                        StatusId = pe.StatusId,
                        //IsDeleted = pe.IsDeleted,
                        PermissionContents = PermissionContentDAOs
                            .Where(pc => pc.PermissionId == pe.Id)
                            .Select(pct => new PermissionContent
                            {
                                Id = pct.Id,
                                PermissionId = pct.PermissionId,
                                FieldId = pct.FieldId,
                                PermissionOperatorId = pct.PermissionOperatorId,
                                Value = pct.Value,
                                //IsDeleted = pct.IsDeleted,
                            }).ToList(),
                        PermissionActionMappings = PermissionActionMappingDAOs
                            .Where(pa => pa.PermissionId == pe.Id)
                            .Select(pam => new PermissionActionMapping
                            {
                                PermissionId = pam.PermissionId,
                                ActionId = pam.ActionId
                            }).ToList()
                    }).ToList(),
                    AppUserRoleMappings = AppUserRoleMappingDAOs.Where(a => a.RoleId == x.Id).Select(ar => new AppUserRoleMapping
                    {
                        AppUserId = ar.AppUserId,
                        RoleId = ar.RoleId
                    }).ToList()
                }).ToList();
            return Roles;
        }
        public async Task<Permission> Get(long Id)
        {
            Permission Permission = await DataContext.Permission.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Permission()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    RoleId = x.RoleId,
                    MenuId = x.MenuId,
                    StatusId = x.StatusId,
                    //StartAt = x.StartAt,
                    //EndAt = x.EndAt,
                    //Status = x.Status == null ? null : new Status
                    //{
                    //    Id = x.Status.Id,
                    //    Code = x.Status.Code,
                    //    Name = x.Status.Name,
                    //},
                    Menu = x.Menu == null ? null : new Menu
                    {
                        Id = x.Menu.Id,
                        Code = x.Menu.Code,
                        Name = x.Menu.Name,
                        IsDeleted = x.Menu.IsDeleted,
                        SiteId = x.Menu.SiteId,
                        //Site = x.Menu.Site == null ? null : new Site
                        //{
                        //    Id = x.Menu.Site.Id,
                        //    Code = x.Menu.Site.Code,
                        //    Name = x.Menu.Site.Name,
                        //    Description = x.Menu.Site.Description,
                        //    IsDisplay = x.Menu.Site.IsDisplay,
                        //    RowId = x.Menu.Site.RowId,
                        //}
                    },
                    Role = x.Role == null ? null : new Role
                    {
                        Id = x.Role.Id,
                        Code = x.Role.Code,
                        Name = x.Role.Name,
                        StatusId = x.Role.StatusId,
                    },
                }).FirstOrDefaultAsync();

            if (Permission == null)
                return null;
            Permission.PermissionContents = await DataContext.PermissionContent
                .Where(x => x.PermissionId == Permission.Id)
                .Select(x => new PermissionContent
                {
                    Id = x.Id,
                    PermissionId = x.PermissionId,
                    FieldId = x.FieldId,
                    PermissionOperatorId = x.PermissionOperatorId,
                    Value = x.Value,
                    Field = new Field
                    {
                        Id = x.Field.Id,
                        Name = x.Field.Name,
                        FieldTypeId = x.Field.FieldTypeId,
                        MenuId = x.Field.MenuId,
                        IsDeleted = x.Field.IsDeleted,
                    },
                    PermissionOperator = new PermissionOperator
                    {
                        Id = x.PermissionOperator.Id,
                        Code = x.PermissionOperator.Code,
                        Name = x.PermissionOperator.Name,
                    },
                }).ToListAsync();

            Permission.PermissionActionMappings = await DataContext.PermissionActionMapping
                .Where(x => x.PermissionId == Permission.Id)
                .Select(x => new PermissionActionMapping
                {
                    PermissionId = x.PermissionId,
                    ActionId = x.ActionId,
                    Action = x.Action == null ? null : new Action
                    {
                        Id = x.Action.Id,
                        Name = x.Action.Name,
                        MenuId = x.Action.MenuId,
                    },
                }).ToListAsync();

            //var FieldFieldEntityMappings = await DataContext.FieldFieldEntityMapping.AsNoTracking().Select(x => new FieldFieldEntityMapping
            //{
            //    FieldEntityId = x.FieldEntityId,
            //    FieldId = x.FieldId,
            //    FieldEntity = new FieldEntity
            //    {
            //        EntityName = x.FieldEntity.EntityName,
            //        EntityDisplay = x.FieldEntity.EntityDisplay,
            //        EntityKey = x.FieldEntity.EntityKey,
            //    }
            //}).ToListAsync();

            //foreach (var content in Permission.PermissionContents)
            //{
            //    content.Field.FieldFieldEntityMappings = FieldFieldEntityMappings.Where(x => x.FieldId == content.FieldId).ToList();
            //}

            return Permission;
        }
        public async Task<bool> Create(Permission Permission)
        {
            PermissionDAO PermissionDAO = new PermissionDAO();
            PermissionDAO.Code = Permission.Code;
            PermissionDAO.Name = Permission.Name;
            PermissionDAO.RoleId = Permission.RoleId;
            PermissionDAO.MenuId = Permission.MenuId;
            //PermissionDAO.StartAt = Permission.StartAt;
            //PermissionDAO.EndAt = Permission.EndAt;
            PermissionDAO.StatusId = Permission.StatusId;
            //PermissionDAO.IsDeleted = false;
            DataContext.Permission.Add(PermissionDAO);
            await DataContext.SaveChangesAsync();
            Permission.Id = PermissionDAO.Id;
            await SaveReference(Permission);
            return true;
        }

        public async Task<bool> Update(Permission Permission)
        {
            PermissionDAO PermissionDAO = DataContext.Permission.Where(x => x.Id == Permission.Id).FirstOrDefault();
            if (PermissionDAO == null)
                return false;
            PermissionDAO.Id = Permission.Id;
            PermissionDAO.Code = Permission.Code;
            PermissionDAO.Name = Permission.Name;
            PermissionDAO.RoleId = Permission.RoleId;
            PermissionDAO.MenuId = Permission.MenuId;
            //PermissionDAO.StartAt = Permission.StartAt;
            //PermissionDAO.EndAt = Permission.EndAt;
            PermissionDAO.StatusId = Permission.StatusId;
            //PermissionDAO.IsDeleted = Permission.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(Permission);
            return true;
        }

        public async Task<List<Role>> Delete(Permission Permission)
        {
            List<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking()
                   .Where(x => x.Id == Permission.RoleId)
                   .ToList();
            List<PermissionDAO> PermissionDAOs = DataContext.Permission.AsNoTracking().ToList();
            List<PermissionContentDAO> PermissionContentDAOs = DataContext.PermissionContent.AsNoTracking().ToList();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = DataContext.PermissionActionMapping.AsNoTracking().ToList();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = DataContext.AppUserRoleMapping.AsNoTracking().ToList();
            List<Role> Roles = RoleDAOs.Select(x => new Role
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                IsDeleted = x.IsDeleted,
                Permissions = PermissionDAOs.Where(p => p.RoleId == x.Id).Select(pe => new Permission
                {
                    Id = pe.Id,
                    Code = pe.Code,
                    Name = pe.Name,
                    RoleId = pe.RoleId,
                    MenuId = pe.MenuId,
                    StatusId = pe.StatusId,
                    //IsDeleted = pe.Id == Permission.Id ? true : pe.IsDeleted,
                    PermissionContents = PermissionContentDAOs
                        .Where(pc => pc.PermissionId == pe.Id)
                        .Select(pct => new PermissionContent
                        {
                            Id = pct.Id,
                            PermissionId = pct.PermissionId,
                            FieldId = pct.FieldId,
                            PermissionOperatorId = pct.PermissionOperatorId,
                            Value = pct.Value,
                            //IsDeleted = pct.PermissionId == Permission.Id ? true : pct.IsDeleted,
                        }).ToList(),
                    PermissionActionMappings = PermissionActionMappingDAOs
                        .Where(pa => pa.PermissionId == pe.Id)
                        .Select(pam => new PermissionActionMapping
                        {
                            PermissionId = pam.PermissionId,
                            ActionId = pam.ActionId
                        }).ToList()
                }).ToList(),
                AppUserRoleMappings = AppUserRoleMappingDAOs.Where(a => a.RoleId == x.Id).Select(ar => new AppUserRoleMapping
                {
                    AppUserId = ar.AppUserId,
                    RoleId = ar.RoleId
                }).ToList()
            }).ToList();
            await DataContext.PermissionContent.Where(x => x.PermissionId == Permission.Id).DeleteFromQueryAsync();
            await DataContext.PermissionActionMapping.Where(x => x.PermissionId == Permission.Id).DeleteFromQueryAsync();
            await DataContext.Permission.Where(x => x.Id == Permission.Id).DeleteFromQueryAsync();
            return Roles;
        }

        public async Task<bool> BulkMerge(List<Permission> Permissions)
        {
            List<PermissionDAO> PermissionDAOs = new List<PermissionDAO>();
            foreach (Permission Permission in Permissions)
            {
                PermissionDAO PermissionDAO = new PermissionDAO();
                PermissionDAO.Code = Permission.Code;
                PermissionDAO.Name = Permission.Name;
                PermissionDAO.RoleId = Permission.RoleId;
                PermissionDAO.MenuId = Permission.MenuId;
                PermissionDAO.StatusId = Permission.StatusId;
                //PermissionDAO.IsDeleted = Permission.IsDeleted;
                PermissionDAOs.Add(PermissionDAO);
            }
            await DataContext.BulkMergeAsync(PermissionDAOs);
            foreach (Permission Permission in Permissions)
            {
                PermissionDAO PermissionDAO = PermissionDAOs.Where(p => p.Code == Permission.Code).FirstOrDefault();
                Permission.Id = PermissionDAO.Id;
                Permission.PermissionActionMappings.ForEach(x => x.PermissionId = PermissionDAO.Id);
                Permission.PermissionContents.ForEach(x => x.PermissionId = PermissionDAO.Id);
            }
            List<PermissionActionMapping> PermissionActionMappings = Permissions.SelectMany(x => x.PermissionActionMappings).ToList();
            List<PermissionContent> PermissionContents = Permissions.SelectMany(x => x.PermissionContents).ToList();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = PermissionActionMappings.Select(x => new PermissionActionMappingDAO
            {
                ActionId = x.ActionId,
                PermissionId = x.PermissionId,
            }).ToList();
            List<PermissionContentDAO> PermissionContentDAOs = PermissionContents.Select(x => new PermissionContentDAO
            {
                FieldId = x.FieldId,
                PermissionOperatorId = x.PermissionOperatorId,
                Value = x.Value,
                PermissionId = x.PermissionId,
                //IsDeleted = x.IsDeleted
            }).ToList();
            await DataContext.BulkMergeAsync(PermissionActionMappingDAOs);
            await DataContext.BulkMergeAsync(PermissionContentDAOs);

            return true;
        }

        public async Task<bool> BulkDelete(List<Permission> Permissions)
        {
            List<long> Ids = Permissions.Select(x => x.Id).ToList();
            await DataContext.Permission
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Permission Permission)
        {
            await DataContext.PermissionContent
                .Where(x => x.PermissionId == Permission.Id)
                .DeleteFromQueryAsync();
            List<PermissionContentDAO> PermissionContentDAOs = new List<PermissionContentDAO>();
            if (Permission.PermissionContents != null)
            {
                foreach (PermissionContent PermissionContent in Permission.PermissionContents)
                {
                    PermissionContentDAO PermissionContentDAO = new PermissionContentDAO();
                    PermissionContentDAO.PermissionId = Permission.Id;
                    PermissionContentDAO.FieldId = PermissionContent.FieldId;
                    PermissionContentDAO.PermissionOperatorId = PermissionContent.PermissionOperatorId;
                    PermissionContentDAO.Value = PermissionContent.Value;
                    //PermissionContentDAO.IsDeleted = false;
                    PermissionContentDAOs.Add(PermissionContentDAO);
                }
                await DataContext.PermissionContent.BulkInsertAsync(PermissionContentDAOs);
            }
            await DataContext.PermissionActionMapping
                .Where(x => x.PermissionId == Permission.Id)
                .DeleteFromQueryAsync();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = new List<PermissionActionMappingDAO>();
            if (Permission.PermissionActionMappings != null)
            {
                foreach (PermissionActionMapping PermissionPageMapping in Permission.PermissionActionMappings)
                {
                    PermissionActionMappingDAO PermissionActionMappingDAO = new PermissionActionMappingDAO();
                    PermissionActionMappingDAO.PermissionId = Permission.Id;
                    PermissionActionMappingDAO.ActionId = PermissionPageMapping.ActionId;
                    PermissionActionMappingDAOs.Add(PermissionActionMappingDAO);
                }
                await DataContext.PermissionActionMapping.BulkMergeAsync(PermissionActionMappingDAOs);
            }
        }

        public async Task<List<long>> ListAppUser(string Path)
        {
            List<long> Ids = await (from aurm in DataContext.AppUserRoleMapping
                                    join r in DataContext.Role on aurm.RoleId equals r.Id
                                    join per in DataContext.Permission on r.Id equals per.RoleId
                                    join pam in DataContext.PermissionActionMapping on per.Id equals pam.PermissionId
                                    join apm in DataContext.ActionPageMapping on pam.ActionId equals apm.ActionId
                                    join page in DataContext.Page on apm.PageId equals page.Id
                                    where per.StatusId == 1 && r.StatusId == 1 && page.Path.StartsWith(Path)
                                    select aurm.AppUserId
                                     ).Distinct().ToListAsync();
            return Ids;
        }

        public async Task<List<string>> ListPath(long AppUserId)
        {
            List<string> Paths = await (from aurm in DataContext.AppUserRoleMapping
                                        join r in DataContext.Role on aurm.RoleId equals r.Id
                                        join per in DataContext.Permission on r.Id equals per.RoleId
                                        join pam in DataContext.PermissionActionMapping on per.Id equals pam.PermissionId
                                        join apm in DataContext.ActionPageMapping on pam.ActionId equals apm.ActionId
                                        join page in DataContext.Page on apm.PageId equals page.Id
                                        where per.StatusId == 1 && r.StatusId == 1 && aurm.AppUserId == AppUserId
                                        select page.Path
                                 ).ToListAsync();
            return Paths;
        }

        public async Task<List<CurrentPermission>> ListByUserAndPath(long AppUserId, string Path)
        {
            string keyIds = $"{nameof(PermissionRepository)}.{nameof(PermissionRepository.ListByUserAndPath)}.{AppUserId}.{Path}";
            List<long> Ids = await GetFromCache<List<long>>(keyIds);
            if (Ids == null || Ids.Count ==0)
            {
                Ids = await (from aurm in DataContext.AppUserRoleMapping.Where(x => x.AppUserId == AppUserId)
                             join r in DataContext.Role on aurm.RoleId equals r.Id
                             join per in DataContext.Permission on aurm.RoleId equals per.RoleId
                             join pam in DataContext.PermissionActionMapping on per.Id equals pam.PermissionId
                             join apm in DataContext.ActionPageMapping on pam.ActionId equals apm.ActionId
                             join page in DataContext.Page on apm.PageId equals page.Id
                             where r.StatusId == StatusEnum.ACTIVE.Id && per.StatusId == StatusEnum.ACTIVE.Id &&
                             page.Path == Path
                             select per.Id
                             ).Distinct().ToListAsync();
                await SetToCache(keyIds, Ids);
            }

            string hash = Ids.ToHash();
            string keyEntity = $"{nameof(PermissionRepository)}.{nameof(PermissionRepository.ListByUserAndPath)}.{hash}";
            List<CurrentPermission> CurrentPermissions = await GetFromCache<List<CurrentPermission>>(keyEntity);
            if (CurrentPermissions == null)
            {
                CurrentPermissions = await DataContext.Permission.AsNoTracking()
                    .Where(p => Ids.Contains(p.Id))
                    .Select(x => new CurrentPermission
                    {
                        Id = x.Id,
                        RoleId = x.RoleId,
                        CurrentPermissionContents = x.PermissionContents.Select(pc => new CurrentPermissionContent
                        {
                            FieldId = pc.FieldId,
                            FieldTypeId = pc.Field.FieldTypeId,
                            FieldName = pc.Field.Name,
                            PermissionOperatorId = pc.PermissionOperatorId,
                            Value = pc.Value,

                        }).ToList()
                    }).ToListAsync();
                await SetToCache(keyEntity, CurrentPermissions);
            }
            return CurrentPermissions;
        }
    }
}
