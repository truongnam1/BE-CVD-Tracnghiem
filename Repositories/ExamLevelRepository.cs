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
    public interface IExamLevelRepository
    {
        Task<int> CountAll(ExamLevelFilter ExamLevelFilter);
        Task<int> Count(ExamLevelFilter ExamLevelFilter);
        Task<List<ExamLevel>> List(ExamLevelFilter ExamLevelFilter);
        Task<List<ExamLevel>> List(List<long> Ids);
    }
    public class ExamLevelRepository : IExamLevelRepository
    {
        private DataContext DataContext;
        public ExamLevelRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ExamLevelDAO>> DynamicFilter(IQueryable<ExamLevelDAO> query, ExamLevelFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);

            return query;
        }

        private async Task<IQueryable<ExamLevelDAO>> OrFilter(IQueryable<ExamLevelDAO> query, ExamLevelFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ExamLevelDAO> initQuery = query.Where(q => false);
            foreach (ExamLevelFilter ExamLevelFilter in filter.OrFilter)
            {
                IQueryable<ExamLevelDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ExamLevelFilter.Id);
                queryable = queryable.Where(q => q.Code, ExamLevelFilter.Code);
                queryable = queryable.Where(q => q.Name, ExamLevelFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ExamLevelDAO> DynamicOrder(IQueryable<ExamLevelDAO> query, ExamLevelFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ExamLevelOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ExamLevelOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ExamLevelOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ExamLevelOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ExamLevelOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ExamLevelOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ExamLevel>> DynamicSelect(IQueryable<ExamLevelDAO> query, ExamLevelFilter filter)
        {
            List<ExamLevel> ExamLevels = await query.Select(q => new ExamLevel()
            {
                Id = filter.Selects.Contains(ExamLevelSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ExamLevelSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ExamLevelSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ExamLevels;
        }

        public async Task<int> CountAll(ExamLevelFilter filter)
        {
            IQueryable<ExamLevelDAO> ExamLevelDAOs = DataContext.ExamLevel.AsNoTracking();
            ExamLevelDAOs = await DynamicFilter(ExamLevelDAOs, filter);
            return await ExamLevelDAOs.CountAsync();
        }

        public async Task<int> Count(ExamLevelFilter filter)
        {
            IQueryable<ExamLevelDAO> ExamLevelDAOs = DataContext.ExamLevel.AsNoTracking();
            ExamLevelDAOs = await DynamicFilter(ExamLevelDAOs, filter);
            ExamLevelDAOs = await OrFilter(ExamLevelDAOs, filter);
            return await ExamLevelDAOs.CountAsync();
        }

        public async Task<List<ExamLevel>> List(ExamLevelFilter filter)
        {
            if (filter == null) return new List<ExamLevel>();
            IQueryable<ExamLevelDAO> ExamLevelDAOs = DataContext.ExamLevel.AsNoTracking();
            ExamLevelDAOs = await DynamicFilter(ExamLevelDAOs, filter);
            ExamLevelDAOs = await OrFilter(ExamLevelDAOs, filter);
            ExamLevelDAOs = DynamicOrder(ExamLevelDAOs, filter);
            List<ExamLevel> ExamLevels = await DynamicSelect(ExamLevelDAOs, filter);
            return ExamLevels;
        }

        public async Task<List<ExamLevel>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ExamLevelDAO> query = DataContext.ExamLevel.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ExamLevel> ExamLevels = await query.AsNoTracking()
            .Select(x => new ExamLevel()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return ExamLevels;
        }

    }
}
