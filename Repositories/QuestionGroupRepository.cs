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
    public interface IQuestionGroupRepository
    {
        Task<int> CountAll(QuestionGroupFilter QuestionGroupFilter);
        Task<int> Count(QuestionGroupFilter QuestionGroupFilter);
        Task<List<QuestionGroup>> List(QuestionGroupFilter QuestionGroupFilter);
        Task<List<QuestionGroup>> List(List<long> Ids);
    }
    public class QuestionGroupRepository : IQuestionGroupRepository
    {
        private DataContext DataContext;
        public QuestionGroupRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<QuestionGroupDAO>> DynamicFilter(IQueryable<QuestionGroupDAO> query, QuestionGroupFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);

            return query;
        }

        private async Task<IQueryable<QuestionGroupDAO>> OrFilter(IQueryable<QuestionGroupDAO> query, QuestionGroupFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<QuestionGroupDAO> initQuery = query.Where(q => false);
            foreach (QuestionGroupFilter QuestionGroupFilter in filter.OrFilter)
            {
                IQueryable<QuestionGroupDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, QuestionGroupFilter.Id);
                queryable = queryable.Where(q => q.Code, QuestionGroupFilter.Code);
                queryable = queryable.Where(q => q.Name, QuestionGroupFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<QuestionGroupDAO> DynamicOrder(IQueryable<QuestionGroupDAO> query, QuestionGroupFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case QuestionGroupOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case QuestionGroupOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case QuestionGroupOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case QuestionGroupOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case QuestionGroupOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case QuestionGroupOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<QuestionGroup>> DynamicSelect(IQueryable<QuestionGroupDAO> query, QuestionGroupFilter filter)
        {
            List<QuestionGroup> QuestionGroups = await query.Select(q => new QuestionGroup()
            {
                Id = filter.Selects.Contains(QuestionGroupSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(QuestionGroupSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(QuestionGroupSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return QuestionGroups;
        }

        public async Task<int> CountAll(QuestionGroupFilter filter)
        {
            IQueryable<QuestionGroupDAO> QuestionGroupDAOs = DataContext.QuestionGroup.AsNoTracking();
            QuestionGroupDAOs = await DynamicFilter(QuestionGroupDAOs, filter);
            return await QuestionGroupDAOs.CountAsync();
        }

        public async Task<int> Count(QuestionGroupFilter filter)
        {
            IQueryable<QuestionGroupDAO> QuestionGroupDAOs = DataContext.QuestionGroup.AsNoTracking();
            QuestionGroupDAOs = await DynamicFilter(QuestionGroupDAOs, filter);
            QuestionGroupDAOs = await OrFilter(QuestionGroupDAOs, filter);
            return await QuestionGroupDAOs.CountAsync();
        }

        public async Task<List<QuestionGroup>> List(QuestionGroupFilter filter)
        {
            if (filter == null) return new List<QuestionGroup>();
            IQueryable<QuestionGroupDAO> QuestionGroupDAOs = DataContext.QuestionGroup.AsNoTracking();
            QuestionGroupDAOs = await DynamicFilter(QuestionGroupDAOs, filter);
            QuestionGroupDAOs = await OrFilter(QuestionGroupDAOs, filter);
            QuestionGroupDAOs = DynamicOrder(QuestionGroupDAOs, filter);
            List<QuestionGroup> QuestionGroups = await DynamicSelect(QuestionGroupDAOs, filter);
            return QuestionGroups;
        }

        public async Task<List<QuestionGroup>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<QuestionGroupDAO> query = DataContext.QuestionGroup.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<QuestionGroup> QuestionGroups = await query.AsNoTracking()
            .Select(x => new QuestionGroup()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return QuestionGroups;
        }

    }
}
