using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Tracnghiem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;
using Action = TrueSight.PER.Entities.Action;
using Microsoft.Extensions.Configuration;
using Tracnghiem.Rpc.exam;
using Tracnghiem.Rpc.exam_history;
using Tracnghiem.Rpc.image;
using Tracnghiem.Rpc.question;

namespace Tracnghiem.Repositories
{
    public interface IRoleRepository
    {
        Task<int> Count(RoleFilter RoleFilter);
        Task<List<Role>> List(RoleFilter RoleFilter);
        Task<List<Role>> List(List<long> Ids);
        Task<Role> Get(long Id);
        Task<bool> Create(Role Role);
        Task<bool> Update(Role Role);
        Task<List<Role>> Delete(Role Role);
        Task<List<long>> BulkMerge(List<Role> Roles);
        Task<List<Role>> BulkDelete(List<Role> Roles);
        Task<bool> BulkMerge(Site Site);
        Task<bool> Used(List<long> Ids);
        Task<long> InitAdmin(string SiteCode);
        Task<long> InitUser(string SiteCode);

        Task<List<long>> ListIds(long AppUserId);
    }
    public class RoleRepository : CacheRepository, IRoleRepository
    {
        private DataContext DataContext;
        public RoleRepository(DataContext DataContext, IRedisStore RedisStore, IConfiguration Configuration)
            : base(DataContext, RedisStore, Configuration)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<RoleDAO> DynamicFilter(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.SiteId, filter.SiteId);
            query = query.Distinct();
            if (filter.Search != null)
                query = query.Where(q =>
                q.Code.ToLower().Contains(filter.Search.ToLower()) ||
                q.Name.ToLower().Contains(filter.Search.ToLower()));
            return query;
        }

        private IQueryable<RoleDAO> OrFilter(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<RoleDAO> initQuery = query.Where(q => false);
            foreach (RoleFilter RoleFilter in filter.OrFilter)
            {
                IQueryable<RoleDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, RoleFilter.Id);
                queryable = queryable.Where(q => q.Code, RoleFilter.Code);
                queryable = queryable.Where(q => q.Name, RoleFilter.Name);
                queryable = queryable.Where(q => q.StatusId, RoleFilter.StatusId);
                queryable = queryable.Where(q => q.SiteId, RoleFilter.SiteId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<RoleDAO> DynamicOrder(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case RoleOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case RoleOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case RoleOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case RoleOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case RoleOrder.Site:
                            query = query.OrderBy(q => q.SiteId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case RoleOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case RoleOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case RoleOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case RoleOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case RoleOrder.Site:
                            query = query.OrderByDescending(q => q.SiteId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Role>> DynamicSelect(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            List<Role> Roles = await query.Select(q => new Role()
            {
                Id = filter.Selects.Contains(RoleSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(RoleSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(RoleSelect.Name) ? q.Name : default(string),
                //StartAt = filter.Selects.Contains(RoleSelect.StartAt) ? q.StartAt : default(DateTime),
                //EndAt = filter.Selects.Contains(RoleSelect.EndAt) ? q.EndAt : default(DateTime),
                StatusId = filter.Selects.Contains(RoleSelect.Status) ? q.StatusId : default(long),
                SiteId = filter.Selects.Contains(RoleSelect.Site) ? q.SiteId : default(long?),
                IsDeleted = filter.Selects.Contains(RoleSelect.IsDeleted) ? q.IsDeleted : default(bool),
            }).ToListAsync();
            List<AppUserRoleMapping> AppUserRoleMappings = await DataContext.AppUserRoleMapping.AsNoTracking()
                .Where(x => x.RoleId, new IdFilter { In = Roles.Select(x => x.Id).ToList() })
                .Select(x => new AppUserRoleMapping
                {
                    RoleId = x.RoleId,
                    AppUserId = x.AppUserId,
                }).ToListAsync();
            foreach (var Role in Roles)
            {
                Role.AppUserRoleMappings = AppUserRoleMappings.Where(x => x.RoleId == Role.Id).ToList();
            }
            return Roles;
        }

        public async Task<int> Count(RoleFilter filter)
        {
            IQueryable<RoleDAO> Roles = DataContext.Role.AsNoTracking();
            Roles = DynamicFilter(Roles, filter);
            Roles = OrFilter(Roles, filter);
            return await Roles.CountAsync();
        }

        public async Task<List<Role>> List(RoleFilter filter)
        {
            if (filter == null) return new List<Role>();
            IQueryable<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking();
            RoleDAOs = DynamicFilter(RoleDAOs, filter);
            RoleDAOs = OrFilter(RoleDAOs, filter);
            RoleDAOs = DynamicOrder(RoleDAOs, filter);
            List<Role> Roles = await DynamicSelect(RoleDAOs, filter);
            return Roles;
        }
        public async Task<List<Role>> List(List<long> Ids)
        {
            List<RoleDAO> RoleDAOs = await DataContext.Role.AsNoTracking()
                .Where(x => Ids.Contains(x.Id))
                .ToListAsync();
            List<PermissionDAO> PermissionDAOs = await DataContext.Permission.AsNoTracking()
                .Where(x => Ids.Contains(x.RoleId))
                .ToListAsync();
            List<PermissionContentDAO> PermissionContentDAOs = await DataContext.PermissionContent.AsNoTracking().ToListAsync();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = await DataContext.PermissionActionMapping.AsNoTracking().ToListAsync();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = await DataContext.AppUserRoleMapping.AsNoTracking().ToListAsync();
            List<ActionDAO> ActionDAOs = await DataContext.Action.AsNoTracking().ToListAsync();
            List<FieldDAO> FieldDAOs = await DataContext.Field.AsNoTracking().ToListAsync();
            List<MenuDAO> MenuDAOs = await DataContext.Menu.ToListAsync();
            List<SiteDAO> SiteDAOs = await DataContext.Site.ToListAsync();
            List<Role> Roles = RoleDAOs.Select(x => new Role
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                SiteId = x.SiteId,
                IsDeleted = x.IsDeleted,
                //Status = x.Status == null ? null : new Status
                //{
                //    Id = x.Status.Id,
                //    Code = x.Status.Code,
                //    Name = x.Status.Name,
                //},
                Site = SiteDAOs.Where(s => s.Id == x.SiteId).Select(x => new Site
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                }).FirstOrDefault(),
                Permissions = PermissionDAOs.Where(p => p.RoleId == x.Id).Select(pe => new Permission
                {
                    Id = pe.Id,
                    Code = pe.Code,
                    Name = pe.Name,
                    RoleId = pe.RoleId,
                    MenuId = pe.MenuId,
                    StatusId = pe.StatusId,
                    //IsDeleted = pe.IsDeleted,
                    Role = new Role
                    {
                        Code = x.Code,
                    },
                    Menu = MenuDAOs.Where(x => x.Id == pe.MenuId)
                    .Select(x => new Menu
                    {
                        Code = x.Code,
                    }).FirstOrDefault(),
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
                            Field = FieldDAOs.Where(x => x.Id == pct.FieldId).Select(x => new Field
                            {
                                Name = x.Name,
                                MenuId = x.MenuId,
                            }).FirstOrDefault()
                        }).ToList(),
                    PermissionActionMappings = PermissionActionMappingDAOs
                        .Where(pa => pa.PermissionId == pe.Id)
                        .Select(pam => new PermissionActionMapping
                        {
                            PermissionId = pam.PermissionId,
                            ActionId = pam.ActionId,
                            Action = ActionDAOs.Where(x => x.Id == pam.ActionId)
                            .Select(x => new Action
                            {
                                Name = x.Name,
                                MenuId = x.MenuId
                            }).FirstOrDefault(),
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
        public async Task<Role> Get(long Id)
        {
            Role Role = await DataContext.Role.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Role()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    //StartAt = x.StartAt,
                    //EndAt = x.EndAt,
                    StatusId = x.StatusId,
                    SiteId = x.SiteId,
                    IsDeleted = x.IsDeleted,
                    //Site = x.Site == null ? null : new Site
                    //{
                    //    Id = x.Site.Id,
                    //    Code = x.Site.Code,
                    //    Name = x.Site.Name
                    //}
                }).FirstOrDefaultAsync();
            if (Role == null)
                return null;
            Role.AppUserRoleMappings = await DataContext.AppUserRoleMapping
                .Where(x => x.RoleId == Role.Id)
                .Select(x => new AppUserRoleMapping
                {
                    AppUserId = x.AppUserId,
                    RoleId = x.RoleId,
                }).ToListAsync();
            Role.Permissions = await DataContext.Permission.Where(x => x.RoleId == Role.Id).Select(x => new Permission
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                MenuId = x.MenuId,
            }).ToListAsync();
            return Role;
        }
        public async Task<bool> Create(Role Role)
        {
            RoleDAO RoleDAO = new RoleDAO();
            RoleDAO.Id = Role.Id;
            RoleDAO.SiteId = Role.SiteId.Value;
            RoleDAO.Code = Role.Code;
            RoleDAO.Name = Role.Name;
            //RoleDAO.StartAt = Role.StartAt;
            //RoleDAO.EndAt = Role.EndAt;
            RoleDAO.StatusId = Role.StatusId;
            RoleDAO.IsDeleted = false;
            DataContext.Role.Add(RoleDAO);
            await DataContext.SaveChangesAsync();
            Role.Id = RoleDAO.Id;
            await SaveReference(Role);
            return true;
        }

        public async Task<bool> Update(Role Role)
        {
            RoleDAO RoleDAO = DataContext.Role.Where(x => x.Id == Role.Id).FirstOrDefault();
            if (RoleDAO == null)
                return false;
            RoleDAO.Id = Role.Id;
            RoleDAO.Code = Role.Code;
            RoleDAO.Name = Role.Name;
            //RoleDAO.StartAt = Role.StartAt;
            //RoleDAO.EndAt = Role.EndAt;
            RoleDAO.StatusId = Role.StatusId;
            RoleDAO.SiteId = Role.SiteId.Value;
            RoleDAO.IsDeleted = Role.IsDeleted;
            await DataContext.SaveChangesAsync();
            await SaveReference(Role);
            return true;
        }

        public async Task<List<Role>> Delete(Role Role)
        {
            List<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking()
                .Where(x => x.Id == Role.Id)
                .ToList();
            List<PermissionDAO> PermissionDAOs = await DataContext.Permission.AsNoTracking()
                .Where(x => x.RoleId == Role.Id)
                .ToListAsync();
            List<PermissionContentDAO> PermissionContentDAOs = DataContext.PermissionContent.AsNoTracking().ToList();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = DataContext.PermissionActionMapping.AsNoTracking().ToList();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = DataContext.AppUserRoleMapping.AsNoTracking().ToList();
            List<Role> Roles = RoleDAOs.Select(x => new Role
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                SiteId = x.SiteId,
                IsDeleted = true,
                Site = new Site
                {
                    Code = SiteEnum.SiteEnumList.Where(s => s.Id == x.SiteId).Select(x => x.Code).FirstOrDefault(),
                    Name = SiteEnum.SiteEnumList.Where(s => s.Id == x.SiteId).Select(x => x.Name).FirstOrDefault()
                },
                Permissions = PermissionDAOs.Where(p => p.RoleId == x.Id).Select(pe => new Permission
                {
                    Id = pe.Id,
                    Code = pe.Code,
                    Name = pe.Name,
                    RoleId = pe.RoleId,
                    MenuId = pe.MenuId,
                    StatusId = pe.StatusId,
                    IsDeleted = true,
                    //PermissionContents = PermissionContentDAOs
                    //    .Where(pc => pc.PermissionId == pe.Id)
                    //    .Select(pct => new PermissionContent
                    //    {
                    //        Id = pct.Id,
                    //        PermissionId = pct.PermissionId,
                    //        FieldId = pct.FieldId,
                    //        PermissionOperatorId = pct.PermissionOperatorId,
                    //        Value = pct.Value,
                    //        IsDeleted = true,
                    //    }).ToList(),
                    //PermissionActionMappings = PermissionActionMappingDAOs
                    //    .Where(pa => pa.PermissionId == pe.Id)
                    //    .Select(pam => new PermissionActionMapping
                    //    {
                    //        PermissionId = pam.PermissionId,
                    //        ActionId = pam.ActionId
                    //    }).ToList()
                }).ToList(),
                //AppUserRoleMappings = AppUserRoleMappingDAOs.Where(a => a.RoleId == x.Id).Select(ar => new AppUserRoleMapping
                //{
                //    AppUserId = ar.AppUserId,
                //    RoleId = ar.RoleId
                //}).ToList()
            }).ToList();

            await DataContext.AppUserRoleMapping.Where(x => x.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.PermissionActionMapping.Where(x => x.Permission.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.PermissionContent.Where(x => x.Permission.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.Permission.Where(x => x.RoleId == Role.Id).DeleteFromQueryAsync();
            await DataContext.Role.Where(x => x.Id == Role.Id).DeleteFromQueryAsync();

            // Trả về tất cả thông tin Role bị xóa
            return Roles;
        }

        public async Task<List<long>> BulkMerge(List<Role> Roles)
        {
            List<RoleDAO> RoleDAOs = new List<RoleDAO>();
            foreach (Role Role in Roles)
            {
                RoleDAO RoleDAO = new RoleDAO();
                RoleDAO.Id = Role.Id;
                RoleDAO.Code = Role.Code;
                RoleDAO.Name = Role.Name;
                RoleDAO.StatusId = Role.StatusId;
                RoleDAO.SiteId = Role.SiteId.Value;
                RoleDAOs.Add(RoleDAO);
            }
            await DataContext.BulkMergeAsync(RoleDAOs);
            List<long> Ids = RoleDAOs.Select(x => x.Id).ToList();
            return Ids;
        }

        public async Task<List<Role>> BulkDelete(List<Role> Roles)
        {
            List<long> Ids = Roles.Select(x => x.Id).ToList();
            List<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking()
                .Where(x => Ids.Contains(x.Id))
                .ToList();
            List<PermissionDAO> PermissionDAOs = await DataContext.Permission.AsNoTracking()
                .Where(x => Ids.Contains(x.RoleId))
                .ToListAsync();
            List<PermissionContentDAO> PermissionContentDAOs = DataContext.PermissionContent.AsNoTracking().ToList();
            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = DataContext.PermissionActionMapping.AsNoTracking().ToList();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = DataContext.AppUserRoleMapping.AsNoTracking().ToList();
            List<Role> newRoles = RoleDAOs.Select(x => new Role
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StatusId = x.StatusId,
                SiteId = x.SiteId,
                IsDeleted = true,
                Permissions = PermissionDAOs.Where(p => p.RoleId == x.Id).Select(pe => new Permission
                {
                    Id = pe.Id,
                    Code = pe.Code,
                    Name = pe.Name,
                    RoleId = pe.RoleId,
                    MenuId = pe.MenuId,
                    StatusId = pe.StatusId,
                    IsDeleted = true,
                    PermissionContents = PermissionContentDAOs
                        .Where(pc => pc.PermissionId == pe.Id)
                        .Select(pct => new PermissionContent
                        {
                            Id = pct.Id,
                            PermissionId = pct.PermissionId,
                            FieldId = pct.FieldId,
                            PermissionOperatorId = pct.PermissionOperatorId,
                            Value = pct.Value,
                            IsDeleted = true,
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

            await DataContext.AppUserRoleMapping.Where(x => Ids.Contains(x.RoleId)).DeleteFromQueryAsync();
            await DataContext.PermissionActionMapping.Where(x => Ids.Contains(x.Permission.RoleId)).DeleteFromQueryAsync();
            await DataContext.PermissionContent.Where(x => Ids.Contains(x.Permission.RoleId)).DeleteFromQueryAsync();
            await DataContext.Permission.Where(x => Ids.Contains(x.RoleId)).DeleteFromQueryAsync();
            await DataContext.Role
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return newRoles;
        }

        private async Task SaveReference(Role Role)
        {
            await DataContext.AppUserRoleMapping
                .Where(x => x.RoleId == Role.Id)
                .DeleteFromQueryAsync();
            List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = new List<AppUserRoleMappingDAO>();
            if (Role.AppUserRoleMappings != null)
            {
                foreach (AppUserRoleMapping AppUserRoleMapping in Role.AppUserRoleMappings)
                {
                    AppUserRoleMappingDAO AppUserRoleMappingDAO = new AppUserRoleMappingDAO();
                    AppUserRoleMappingDAO.AppUserId = AppUserRoleMapping.AppUserId;
                    AppUserRoleMappingDAO.RoleId = Role.Id;
                    AppUserRoleMappingDAOs.Add(AppUserRoleMappingDAO);
                }
                await DataContext.AppUserRoleMapping.BulkMergeAsync(AppUserRoleMappingDAOs);
            }
        }

        public async Task<bool> BulkMerge(Site Site)
        {
            #region Role
            List<SiteDAO> SiteDAOs = await DataContext.Site.ToListAsync();
            long SiteId = SiteDAOs.Where(x => x.Code == Site.Code).Select(x => x.Id).FirstOrDefault();

            List<Role> Roles = Site.Roles;
            List<RoleDAO> DbRoleDAOs = await DataContext.Role.Where(x => x.SiteId == SiteId).ToListAsync();
            DbRoleDAOs.ForEach(x => x.IsDeleted = true);
            foreach (var Role in Roles)
            {
                RoleDAO oldRole = DbRoleDAOs
                    .Where(m => m.Id == Role.Id && m.Code == Role.Code)
                    .FirstOrDefault();
                if (oldRole == null)
                {
                    oldRole = new RoleDAO()
                    {
                        Code = Role.Code,
                        Name = Role.Name,
                        StatusId = Role.StatusId,
                        SiteId = SiteId,
                        Used = Role.Used,
                        IsDeleted = false
                    };
                    DbRoleDAOs.Add(oldRole);
                }
                else
                {
                    oldRole.Name = Role.Name;
                    oldRole.StatusId = Role.StatusId;
                    oldRole.Used = Role.Used;
                    oldRole.IsDeleted = false;
                }
            }
            await DataContext.BulkMergeAsync(DbRoleDAOs);
            Roles.ForEach(x => x.Id = DbRoleDAOs.Where(m => m.Code == x.Code && m.IsDeleted == false).Select(m => m.Id).FirstOrDefault());
            #endregion

            List<long> RoleIds = Roles.Select(x => x.Id).ToList();

            #region Permission
            List<MenuDAO> MenuDAOs = await DataContext.Menu.Where(x => x.SiteId == SiteId).ToListAsync();

            List<PermissionDAO> DbPermissionDAOs = await DataContext.Permission
                .Where(x => x.Role.SiteId == SiteId)
                .ToListAsync();
            //DbPermissionDAOs.ForEach(x => x.IsDeleted = true);

            foreach (var Role in Roles)
            {
                long RoleId = Role.Id;
                List<Permission> Permissions = Role.Permissions;
                if (Permissions != null)
                {
                    foreach (Permission Permission in Permissions)
                    {
                        long MenuId = MenuDAOs.Where(x => x.Code == Permission.Menu.Code && x.SiteId == SiteId).Select(m => m.Id).FirstOrDefault();
                        PermissionDAO oldPermission = DbPermissionDAOs
                            .Where(p => p.Id == Permission.Id)
                            .FirstOrDefault();
                        if (oldPermission == null)
                        {
                            oldPermission = new PermissionDAO
                            {
                                Code = Permission.Code,
                                Name = Permission.Name,
                                RoleId = RoleId,
                                StatusId = Permission.StatusId,
                                MenuId = MenuId,
                                //IsDeleted = false,
                            };
                            DbPermissionDAOs.Add(oldPermission);
                        }
                        else
                        {
                            oldPermission.Code = Permission.Code;
                            oldPermission.Name = Permission.Name;
                            oldPermission.RoleId = RoleId;
                            oldPermission.MenuId = MenuId;
                            //oldPermission.IsDeleted = false;
                        }
                    }
                }
            }
            await DataContext.BulkMergeAsync(DbPermissionDAOs);
            //Update Permission
            Roles.ForEach(x =>
            {
                //Permission
                if (x.Permissions != null)
                    x.Permissions.ForEach(p =>
                    {
                        p.Id = DbPermissionDAOs
                    .Where(a => a.Code == p.Code && a.RoleId == x.Id)
                    .Select(a => a.Id).FirstOrDefault();
                        p.MenuId = DbPermissionDAOs
                    .Where(a => a.Code == p.Code && a.RoleId == x.Id)
                    .Select(a => a.MenuId).FirstOrDefault();
                        p.RoleId = x.Id;
                    });
            });
            #endregion

            #region AppUserRoleMapping
            //Delete old AppUserRoleMapping
            await DataContext.AppUserRoleMapping.Where(x => x.Role.SiteId == SiteId).DeleteFromQueryAsync();

            var AppUserRoleMappingDAOs = new List<AppUserRoleMappingDAO>();
            foreach (var Role in Roles)
            {
                long RoleId = Role.Id;
                List<AppUserRoleMapping> AppUserRoleMappings = Role.AppUserRoleMappings;
                if (AppUserRoleMappings != null)
                {
                    foreach (AppUserRoleMapping AppUserRoleMapping in AppUserRoleMappings)
                    {
                        AppUserRoleMappingDAO AppUserRoleMappingDAO = new AppUserRoleMappingDAO
                        {
                            AppUserId = AppUserRoleMapping.AppUserId,
                            RoleId = RoleId
                        };
                        AppUserRoleMappingDAOs.Add(AppUserRoleMappingDAO);
                    }
                }
            }
            List<AppUserRoleMappingDAO> NewAppUserRoleMappingDAOs = new List<AppUserRoleMappingDAO>();
            foreach (var item in AppUserRoleMappingDAOs)
            {
                var AppUserRoleMappingDAO = NewAppUserRoleMappingDAOs
                    .Where(x => x.AppUserId == item.AppUserId && x.RoleId == item.RoleId).FirstOrDefault();
                if (AppUserRoleMappingDAO != null)
                    continue;
                NewAppUserRoleMappingDAOs.Add(new AppUserRoleMappingDAO { AppUserId = item.AppUserId, RoleId = item.RoleId });
            }
            await DataContext.BulkMergeAsync(NewAppUserRoleMappingDAOs);

            #endregion

            #region PermissionActionMapping
            //Delete old PermissionActionMapping
            List<long> PermissionIds = DbPermissionDAOs.Select(x => x.Id).ToList();
            await DataContext.PermissionActionMapping.Where(x => x.Permission.Role.SiteId == SiteId).DeleteFromQueryAsync();

            List<long> MenuIds = MenuDAOs.Select(x => x.Id).ToList();
            List<ActionDAO> ActionDAOs = DataContext.Action.Where(x => x.Menu.SiteId == SiteId).ToList();
            var PermissionActionMappingDAOs = new List<PermissionActionMappingDAO>();
            foreach (var Role in Roles)
            {
                List<Permission> Permissions = Role.Permissions;
                if (Permissions != null)
                {
                    foreach (var Permission in Permissions)
                    {
                        List<PermissionActionMapping> PermissionActionMappings = Permission.PermissionActionMappings;
                        if (PermissionActionMappings != null)
                        {
                            foreach (var pam in PermissionActionMappings)
                            {
                                long ActionId = ActionDAOs.Where(a => pam.Action.Name == a.Name && a.MenuId == Permission.MenuId)
                                    .Select(x => x.Id).FirstOrDefault();
                                PermissionActionMappingDAO PermissionActionMappingDAO = new PermissionActionMappingDAO
                                {
                                    ActionId = ActionId,
                                    PermissionId = Permission.Id
                                };
                                PermissionActionMappingDAOs.Add(PermissionActionMappingDAO);
                            }
                        }
                    }
                }
            }
            List<PermissionActionMappingDAO> SubPermissionActionMappingDAOs = new List<PermissionActionMappingDAO>();
            foreach (var item in PermissionActionMappingDAOs)
            {
                var PermissionActionMappingDAO = SubPermissionActionMappingDAOs
                    .Where(x => x.ActionId == item.ActionId && x.PermissionId == item.PermissionId).FirstOrDefault();
                if (PermissionActionMappingDAO != null) continue;
                SubPermissionActionMappingDAOs.Add(new PermissionActionMappingDAO { ActionId = item.ActionId, PermissionId = item.PermissionId });
            }
            SubPermissionActionMappingDAOs = SubPermissionActionMappingDAOs.Where(x => x.ActionId != 0).ToList();
            await DataContext.BulkMergeAsync(SubPermissionActionMappingDAOs);

            #endregion

            #region PermissionContent
            //Delete old data
            await DataContext.PermissionContent.Where(x => x.Permission.Role.SiteId == SiteId).DeleteFromQueryAsync();

            List<FieldDAO> FieldDAOs = DataContext.Field.Where(x => x.Menu.SiteId == SiteId).ToList();
            var PermissionContentDAOs = new List<PermissionContentDAO>();
            foreach (var Role in Roles)
            {
                List<Permission> Permissions = Role.Permissions;
                if (Permissions != null)
                {
                    foreach (var Permission in Permissions)
                    {
                        List<PermissionContent> PermissionContents = Permission.PermissionContents;
                        if (PermissionContents != null)
                        {
                            foreach (var pc in PermissionContents)
                            {
                                long FieldId = FieldDAOs.Where(f => pc.Field.Name == f.Name && f.MenuId == Permission.MenuId)
                                    .Select(x => x.Id).FirstOrDefault();
                                PermissionContentDAO PermissionContentDAO = new PermissionContentDAO
                                {
                                    FieldId = FieldId,
                                    PermissionId = Permission.Id,
                                    PermissionOperatorId = pc.PermissionOperatorId,
                                    Value = pc.Value,
                                    //IsDeleted = false,
                                };
                                PermissionContentDAOs.Add(PermissionContentDAO);
                            }
                        }
                    }

                }
            }
            await DataContext.BulkMergeAsync(PermissionContentDAOs);

            //await DataContext.PermissionContent.Where(x => x.Permission.IsDeleted).DeleteFromQueryAsync();
            //await DataContext.PermissionActionMapping.Where(x => x.Permission.IsDeleted).DeleteFromQueryAsync();
            //await DataContext.Permission.Where(x => x.IsDeleted).DeleteFromQueryAsync();
            await DataContext.Role.Where(x => x.IsDeleted).DeleteFromQueryAsync();
            #endregion

            return true;
        }

        public async Task<long> InitAdmin(string SiteCode)
        {
            SiteDAO Site = await DataContext.Site.Where(x => x.Code == SiteCode).FirstOrDefaultAsync();
            long SiteId = Site.Id;
            string SiteName = Site.Name.ChangeToEnglishChar().Trim();
            string AdminName = $"{SiteName}_ADMIN";
            RoleDAO AdminRole = await DataContext.Role.AsNoTracking()
               .Where(r => r.Name == AdminName).FirstOrDefaultAsync();

            if (AdminRole == null)
            {
                AdminRole = new RoleDAO
                {
                    Name = $"{SiteName}_ADMIN",
                    Code = $"{SiteName}_ADMIN",
                    StatusId = StatusEnum.ACTIVE.Id,
                    SiteId = SiteId,
                };
                DataContext.Role.Add(AdminRole);
                DataContext.SaveChanges();
            }

            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().Where(x => x.SiteId == SiteId)
                .Include(v => v.Actions)
                .ToList();
            List<PermissionDAO> permissions = DataContext.Permission.AsNoTracking().Where(x => x.Role.SiteId == SiteId)
                .Include(p => p.PermissionActionMappings)
                .ToList();
            foreach (MenuDAO Menu in Menus)
            {
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == AdminRole.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
                        Code = $"{Site.Name}.{AdminRole.Code}" + "_" + Menu.Name,
                        Name = $"{Site.Name}.{AdminRole.Code}" + "_" + Menu.Name,
                        MenuId = Menu.Id,
                        RoleId = AdminRole.Id,
                        StatusId = StatusEnum.ACTIVE.Id,
                        PermissionActionMappings = new List<PermissionActionMappingDAO>(),
                    };
                    permissions.Add(permission);
                }
                else
                {
                    permission.StatusId = StatusEnum.ACTIVE.Id;
                    if (permission.PermissionActionMappings == null)
                        permission.PermissionActionMappings = new List<PermissionActionMappingDAO>();
                }
                foreach (ActionDAO action in Menu.Actions)
                {
                    PermissionActionMappingDAO PermissionActionMappingDAO = permission.PermissionActionMappings
                        .Where(ppm => ppm.ActionId == action.Id).FirstOrDefault();
                    if (PermissionActionMappingDAO == null)
                    {
                        PermissionActionMappingDAO = new PermissionActionMappingDAO
                        {
                            ActionId = action.Id
                        };
                        permission.PermissionActionMappings.Add(PermissionActionMappingDAO);
                    }
                }

            }
            DataContext.Permission.BulkMerge(permissions);
            permissions.ForEach(p =>
            {
                foreach (var action in p.PermissionActionMappings)
                {
                    action.PermissionId = p.Id;
                }
            });

            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = permissions
                .SelectMany(p => p.PermissionActionMappings).ToList();
            DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == AdminRole.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == AdminRole.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.BulkMerge(PermissionActionMappingDAOs);

            AppUserDAO SiteAdmin = await DataContext.AppUser
                .Where(au => au.Username.ToLower() == $"{SiteName}_Administrator".ToLower() && au.DeletedAt == null)
                .FirstOrDefaultAsync();
            AppUserDAO Admin = await DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower() && au.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (SiteAdmin != null)
            {
                AppUserRoleMappingDAO SiteAdminRoleMappingDAO = await DataContext.AppUserRoleMapping.AsNoTracking()
                    .Where(ur => ur.RoleId == AdminRole.Id && ur.AppUserId == SiteAdmin.Id)
                    .FirstOrDefaultAsync();
                if (SiteAdminRoleMappingDAO == null)
                {
                    SiteAdminRoleMappingDAO = new AppUserRoleMappingDAO
                    {
                        AppUserId = SiteAdmin.Id,
                        RoleId = AdminRole.Id,
                    };
                    DataContext.AppUserRoleMapping.Add(SiteAdminRoleMappingDAO);
                    DataContext.SaveChanges();
                }
            }
            if (Admin != null)
            {
                AppUserRoleMappingDAO AdminRoleMappingDAO = await DataContext.AppUserRoleMapping.AsNoTracking()
                    .Where(ur => ur.RoleId == AdminRole.Id && ur.AppUserId == Admin.Id)
                    .FirstOrDefaultAsync();

                if (AdminRoleMappingDAO == null)
                {
                    AdminRoleMappingDAO = new AppUserRoleMappingDAO
                    {
                        AppUserId = Admin.Id,
                        RoleId = AdminRole.Id,
                    };
                    DataContext.AppUserRoleMapping.Add(AdminRoleMappingDAO);
                    DataContext.SaveChanges();
                }
            }

            return AdminRole.Id;
        }
        public async Task<long> InitUser(string SiteCode)
        {

            Dictionary<string, IEnumerable<string>> UserRoleActionDictionary= new Dictionary<string, IEnumerable<string>>();
            UserRoleActionDictionary.Add(nameof(ExamRoute), new List<string> { "Tìm kiếm", "Thêm", "Sửa", "Xoá"});
            UserRoleActionDictionary.Add(nameof(ExamHistoryRoute), new List<string> { "Tìm kiếm"});
            UserRoleActionDictionary.Add(nameof(ImageRoute), new List<string> { "Tìm kiếm", "Thêm", "Sửa" });
            UserRoleActionDictionary.Add(nameof(QuestionRoute), new List<string> { "Tìm kiếm", "Thêm", "Sửa", "Xoá" });


            SiteDAO Site = await DataContext.Site.Where(x => x.Code == SiteCode).FirstOrDefaultAsync();
            long SiteId = Site.Id;
            string SiteName = Site.Name.ChangeToEnglishChar().Trim();
            string UserRoleName = $"{SiteName}_USER";
            RoleDAO UserRole = await DataContext.Role.AsNoTracking()
               .Where(r => r.Name == UserRoleName).FirstOrDefaultAsync();

            if (UserRole == null)
            {
                UserRole = new RoleDAO
                {
                    Name = RoleEnum.UserRole.Code,
                    Code = RoleEnum.UserRole.Code,
                    StatusId = StatusEnum.ACTIVE.Id,
                    SiteId = SiteId,
                };
                DataContext.Role.Add(UserRole);
                DataContext.SaveChanges();
            }

            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().Where(x => x.SiteId == SiteId)
                .Include(v => v.Actions)
                .ToList();
            List<PermissionDAO> permissions = DataContext.Permission.AsNoTracking().Where(x => x.Role.SiteId == SiteId)
                .Include(p => p.PermissionActionMappings)
                .ToList();
            foreach (MenuDAO Menu in Menus)
            {
                if (!UserRoleActionDictionary.ContainsKey(Menu.Code))
                    continue;
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == UserRole.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
                        Code = $"{Site.Name}.{UserRole.Code}" + "_" + Menu.Name,
                        Name = $"{Site.Name}.{UserRole.Code}" + "_" + Menu.Name,
                        MenuId = Menu.Id,
                        RoleId = UserRole.Id,
                        StatusId = StatusEnum.ACTIVE.Id,
                        PermissionActionMappings = new List<PermissionActionMappingDAO>(),
                    };
                    permissions.Add(permission);
                }
                else
                {
                    permission.StatusId = StatusEnum.ACTIVE.Id;
                    if (permission.PermissionActionMappings == null)
                        permission.PermissionActionMappings = new List<PermissionActionMappingDAO>();
                }
                foreach (ActionDAO action in Menu.Actions)
                {
                    PermissionActionMappingDAO PermissionActionMappingDAO = permission.PermissionActionMappings
                        .Where(ppm => ppm.ActionId == action.Id).FirstOrDefault();
                    ActionDAO Action = await DataContext.Action.Where(x => x.Id == action.Id).FirstOrDefaultAsync();
                    if (!UserRoleActionDictionary[Menu.Code].Contains(Action.Name))
                        continue;
                    if (PermissionActionMappingDAO == null)
                    {
                        PermissionActionMappingDAO = new PermissionActionMappingDAO
                        {
                            ActionId = action.Id
                        };
                        permission.PermissionActionMappings.Add(PermissionActionMappingDAO);
                    }
                }

            }
            DataContext.Permission.BulkMerge(permissions);
            permissions.ForEach(p =>
            {
                foreach (var action in p.PermissionActionMappings)
                {
                    action.PermissionId = p.Id;
                }
            });

            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = permissions
                .SelectMany(p => p.PermissionActionMappings).ToList();
            DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == UserRole.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == UserRole.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.BulkMerge(PermissionActionMappingDAOs);

            return 1;
        }

        public async Task<bool> Used(List<long> Ids)
        {
            await DataContext.Role.Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new RoleDAO { Used = true });
            return true;
        }

        public async Task<List<long>> ListIds(long AppUserId)
        {
            string key = $"{nameof(PermissionRepository)}.{nameof(RoleRepository.ListIds)}.{AppUserId}";
            List<long> Ids = await GetFromCache<List<long>>(key);
            if (Ids == null)
            {
                Ids = await DataContext.AppUserRoleMapping.Where(x => x.AppUserId == AppUserId)
                        .Select(x => x.RoleId).ToListAsync();
                await SetToCache(key, Ids);
            }
            return Ids;
        }
        private async Task RemoveCache()
        {
            await RemoveFromCache(nameof(PermissionRepository));
            await RemoveFromCache(nameof(RoleRepository));
        }

    }
}
