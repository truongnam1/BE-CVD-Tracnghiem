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
    public interface ISubjectRepository
    {
        Task<int> CountAll(SubjectFilter SubjectFilter);
        Task<int> Count(SubjectFilter SubjectFilter);
        Task<List<Subject>> List(SubjectFilter SubjectFilter);
        Task<List<Subject>> List(List<long> Ids);
    }
    public class SubjectRepository : ISubjectRepository
    {
        private DataContext DataContext;
        public SubjectRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<SubjectDAO>> DynamicFilter(IQueryable<SubjectDAO> query, SubjectFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            if (filter.Search != null)
            {
                query = query.Where(q =>
                   q.Code.ToLower().Contains(filter.Search.Trim().ToLower()) ||
                   q.Name.ToLower().Contains(filter.Search.Trim().ToLower()));
            }
            return query;
        }

        private async Task<IQueryable<SubjectDAO>> OrFilter(IQueryable<SubjectDAO> query, SubjectFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SubjectDAO> initQuery = query.Where(q => false);
            foreach (SubjectFilter SubjectFilter in filter.OrFilter)
            {
                IQueryable<SubjectDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, SubjectFilter.Id);
                queryable = queryable.Where(q => q.Code, SubjectFilter.Code);
                queryable = queryable.Where(q => q.Name, SubjectFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<SubjectDAO> DynamicOrder(IQueryable<SubjectDAO> query, SubjectFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SubjectOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SubjectOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case SubjectOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SubjectOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SubjectOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case SubjectOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Subject>> DynamicSelect(IQueryable<SubjectDAO> query, SubjectFilter filter)
        {
            List<Subject> Subjects = await query.Select(q => new Subject()
            {
                Id = filter.Selects.Contains(SubjectSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(SubjectSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(SubjectSelect.Name) ? q.Name : default(string),
                Avatar = filter.Selects.Contains(SubjectSelect.Avatar) ? q.Avatar : default(string),
            }).ToListAsync();
            return Subjects;
        }

        public async Task<int> CountAll(SubjectFilter filter)
        {
            IQueryable<SubjectDAO> SubjectDAOs = DataContext.Subject.AsNoTracking();
            SubjectDAOs = await DynamicFilter(SubjectDAOs, filter);
            return await SubjectDAOs.CountAsync();
        }

        public async Task<int> Count(SubjectFilter filter)
        {
            IQueryable<SubjectDAO> SubjectDAOs = DataContext.Subject.AsNoTracking();
            SubjectDAOs = await DynamicFilter(SubjectDAOs, filter);
            SubjectDAOs = await OrFilter(SubjectDAOs, filter);
            return await SubjectDAOs.CountAsync();
        }

        public async Task<List<Subject>> List(SubjectFilter filter)
        {
            if (filter == null) return new List<Subject>();
            IQueryable<SubjectDAO> SubjectDAOs = DataContext.Subject.AsNoTracking();
            SubjectDAOs = await DynamicFilter(SubjectDAOs, filter);
            SubjectDAOs = await OrFilter(SubjectDAOs, filter);
            SubjectDAOs = DynamicOrder(SubjectDAOs, filter);
            List<Subject> Subjects = await DynamicSelect(SubjectDAOs, filter);
            return Subjects;
        }

        public async Task<List<Subject>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<SubjectDAO> query = DataContext.Subject.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Subject> Subjects = await query.AsNoTracking()
            .Select(x => new Subject()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Avatar = x.Avatar,
            }).ToListAsync();
            

            return Subjects;
        }

    }
}
