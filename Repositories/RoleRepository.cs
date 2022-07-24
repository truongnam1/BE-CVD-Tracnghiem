using TrueSight.Common;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using Tracnghiem.Entities;
using Tracnghiem.Models;
using Tracnghiem.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace Tracnghiem.Repositories
{
    public interface IRoleRepository
    {
        Task<int> CountAll(RoleFilter RoleFilter);
        Task<int> Count(RoleFilter RoleFilter);
        Task<List<Role>> List(RoleFilter RoleFilter);
        Task<List<Role>> List(List<long> Ids);
    }
    public class RoleRepository : IRoleRepository
    {
        private DataContext DataContext;
        public RoleRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<RoleDAO>> DynamicFilter(IQueryable<RoleDAO> query, RoleFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);

            return query;
        }

        private async Task<IQueryable<RoleDAO>> OrFilter(IQueryable<RoleDAO> query, RoleFilter filter)
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
            }).ToListAsync();
            return Roles;
        }

        public async Task<int> CountAll(RoleFilter filter)
        {
            IQueryable<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking();
            RoleDAOs = await DynamicFilter(RoleDAOs, filter);
            return await RoleDAOs.CountAsync();
        }

        public async Task<int> Count(RoleFilter filter)
        {
            IQueryable<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking();
            RoleDAOs = await DynamicFilter(RoleDAOs, filter);
            RoleDAOs = await OrFilter(RoleDAOs, filter);
            return await RoleDAOs.CountAsync();
        }

        public async Task<List<Role>> List(RoleFilter filter)
        {
            if (filter == null) return new List<Role>();
            IQueryable<RoleDAO> RoleDAOs = DataContext.Role.AsNoTracking();
            RoleDAOs = await DynamicFilter(RoleDAOs, filter);
            RoleDAOs = await OrFilter(RoleDAOs, filter);
            RoleDAOs = DynamicOrder(RoleDAOs, filter);
            List<Role> Roles = await DynamicSelect(RoleDAOs, filter);
            return Roles;
        }

        public async Task<List<Role>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<RoleDAO> query = DataContext.Role.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Role> Roles = await query.AsNoTracking()
            .Select(x => new Role()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return Roles;
        }

    }
}
