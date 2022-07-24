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
    public interface IExamStatusRepository
    {
        Task<int> CountAll(ExamStatusFilter ExamStatusFilter);
        Task<int> Count(ExamStatusFilter ExamStatusFilter);
        Task<List<ExamStatus>> List(ExamStatusFilter ExamStatusFilter);
        Task<List<ExamStatus>> List(List<long> Ids);
    }
    public class ExamStatusRepository : IExamStatusRepository
    {
        private DataContext DataContext;
        public ExamStatusRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ExamStatusDAO>> DynamicFilter(IQueryable<ExamStatusDAO> query, ExamStatusFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);

            return query;
        }

        private async Task<IQueryable<ExamStatusDAO>> OrFilter(IQueryable<ExamStatusDAO> query, ExamStatusFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ExamStatusDAO> initQuery = query.Where(q => false);
            foreach (ExamStatusFilter ExamStatusFilter in filter.OrFilter)
            {
                IQueryable<ExamStatusDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ExamStatusFilter.Id);
                queryable = queryable.Where(q => q.Code, ExamStatusFilter.Code);
                queryable = queryable.Where(q => q.Name, ExamStatusFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ExamStatusDAO> DynamicOrder(IQueryable<ExamStatusDAO> query, ExamStatusFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ExamStatusOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ExamStatusOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ExamStatusOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ExamStatusOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ExamStatusOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ExamStatusOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ExamStatus>> DynamicSelect(IQueryable<ExamStatusDAO> query, ExamStatusFilter filter)
        {
            List<ExamStatus> ExamStatuses = await query.Select(q => new ExamStatus()
            {
                Id = filter.Selects.Contains(ExamStatusSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ExamStatusSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ExamStatusSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return ExamStatuses;
        }

        public async Task<int> CountAll(ExamStatusFilter filter)
        {
            IQueryable<ExamStatusDAO> ExamStatusDAOs = DataContext.ExamStatus.AsNoTracking();
            ExamStatusDAOs = await DynamicFilter(ExamStatusDAOs, filter);
            return await ExamStatusDAOs.CountAsync();
        }

        public async Task<int> Count(ExamStatusFilter filter)
        {
            IQueryable<ExamStatusDAO> ExamStatusDAOs = DataContext.ExamStatus.AsNoTracking();
            ExamStatusDAOs = await DynamicFilter(ExamStatusDAOs, filter);
            ExamStatusDAOs = await OrFilter(ExamStatusDAOs, filter);
            return await ExamStatusDAOs.CountAsync();
        }

        public async Task<List<ExamStatus>> List(ExamStatusFilter filter)
        {
            if (filter == null) return new List<ExamStatus>();
            IQueryable<ExamStatusDAO> ExamStatusDAOs = DataContext.ExamStatus.AsNoTracking();
            ExamStatusDAOs = await DynamicFilter(ExamStatusDAOs, filter);
            ExamStatusDAOs = await OrFilter(ExamStatusDAOs, filter);
            ExamStatusDAOs = DynamicOrder(ExamStatusDAOs, filter);
            List<ExamStatus> ExamStatuses = await DynamicSelect(ExamStatusDAOs, filter);
            return ExamStatuses;
        }

        public async Task<List<ExamStatus>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ExamStatusDAO> query = DataContext.ExamStatus.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ExamStatus> ExamStatuses = await query.AsNoTracking()
            .Select(x => new ExamStatus()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return ExamStatuses;
        }

    }
}
