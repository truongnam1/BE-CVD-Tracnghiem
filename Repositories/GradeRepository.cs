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
    public interface IGradeRepository
    {
        Task<int> CountAll(GradeFilter GradeFilter);
        Task<int> Count(GradeFilter GradeFilter);
        Task<List<Grade>> List(GradeFilter GradeFilter);
        Task<List<Grade>> List(List<long> Ids);
    }
    public class GradeRepository : IGradeRepository
    {
        private DataContext DataContext;
        public GradeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<GradeDAO>> DynamicFilter(IQueryable<GradeDAO> query, GradeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);

            return query;
        }

        private async Task<IQueryable<GradeDAO>> OrFilter(IQueryable<GradeDAO> query, GradeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<GradeDAO> initQuery = query.Where(q => false);
            foreach (GradeFilter GradeFilter in filter.OrFilter)
            {
                IQueryable<GradeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, GradeFilter.Id);
                queryable = queryable.Where(q => q.Code, GradeFilter.Code);
                queryable = queryable.Where(q => q.Name, GradeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<GradeDAO> DynamicOrder(IQueryable<GradeDAO> query, GradeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case GradeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case GradeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case GradeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case GradeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case GradeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case GradeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Grade>> DynamicSelect(IQueryable<GradeDAO> query, GradeFilter filter)
        {
            List<Grade> Grades = await query.Select(q => new Grade()
            {
                Id = filter.Selects.Contains(GradeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(GradeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(GradeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return Grades;
        }

        public async Task<int> CountAll(GradeFilter filter)
        {
            IQueryable<GradeDAO> GradeDAOs = DataContext.Grade.AsNoTracking();
            GradeDAOs = await DynamicFilter(GradeDAOs, filter);
            return await GradeDAOs.CountAsync();
        }

        public async Task<int> Count(GradeFilter filter)
        {
            IQueryable<GradeDAO> GradeDAOs = DataContext.Grade.AsNoTracking();
            GradeDAOs = await DynamicFilter(GradeDAOs, filter);
            GradeDAOs = await OrFilter(GradeDAOs, filter);
            return await GradeDAOs.CountAsync();
        }

        public async Task<List<Grade>> List(GradeFilter filter)
        {
            if (filter == null) return new List<Grade>();
            IQueryable<GradeDAO> GradeDAOs = DataContext.Grade.AsNoTracking();
            GradeDAOs = await DynamicFilter(GradeDAOs, filter);
            GradeDAOs = await OrFilter(GradeDAOs, filter);
            GradeDAOs = DynamicOrder(GradeDAOs, filter);
            List<Grade> Grades = await DynamicSelect(GradeDAOs, filter);
            return Grades;
        }

        public async Task<List<Grade>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<GradeDAO> query = DataContext.Grade.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Grade> Grades = await query.AsNoTracking()
            .Select(x => new Grade()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return Grades;
        }

    }
}
