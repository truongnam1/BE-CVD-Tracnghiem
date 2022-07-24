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
    public interface IQuestionTypeRepository
    {
        Task<int> CountAll(QuestionTypeFilter QuestionTypeFilter);
        Task<int> Count(QuestionTypeFilter QuestionTypeFilter);
        Task<List<QuestionType>> List(QuestionTypeFilter QuestionTypeFilter);
        Task<List<QuestionType>> List(List<long> Ids);
    }
    public class QuestionTypeRepository : IQuestionTypeRepository
    {
        private DataContext DataContext;
        public QuestionTypeRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<QuestionTypeDAO>> DynamicFilter(IQueryable<QuestionTypeDAO> query, QuestionTypeFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);

            return query;
        }

        private async Task<IQueryable<QuestionTypeDAO>> OrFilter(IQueryable<QuestionTypeDAO> query, QuestionTypeFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<QuestionTypeDAO> initQuery = query.Where(q => false);
            foreach (QuestionTypeFilter QuestionTypeFilter in filter.OrFilter)
            {
                IQueryable<QuestionTypeDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, QuestionTypeFilter.Id);
                queryable = queryable.Where(q => q.Code, QuestionTypeFilter.Code);
                queryable = queryable.Where(q => q.Name, QuestionTypeFilter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<QuestionTypeDAO> DynamicOrder(IQueryable<QuestionTypeDAO> query, QuestionTypeFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case QuestionTypeOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case QuestionTypeOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case QuestionTypeOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case QuestionTypeOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case QuestionTypeOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case QuestionTypeOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<QuestionType>> DynamicSelect(IQueryable<QuestionTypeDAO> query, QuestionTypeFilter filter)
        {
            List<QuestionType> QuestionTypes = await query.Select(q => new QuestionType()
            {
                Id = filter.Selects.Contains(QuestionTypeSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(QuestionTypeSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(QuestionTypeSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return QuestionTypes;
        }

        public async Task<int> CountAll(QuestionTypeFilter filter)
        {
            IQueryable<QuestionTypeDAO> QuestionTypeDAOs = DataContext.QuestionType.AsNoTracking();
            QuestionTypeDAOs = await DynamicFilter(QuestionTypeDAOs, filter);
            return await QuestionTypeDAOs.CountAsync();
        }

        public async Task<int> Count(QuestionTypeFilter filter)
        {
            IQueryable<QuestionTypeDAO> QuestionTypeDAOs = DataContext.QuestionType.AsNoTracking();
            QuestionTypeDAOs = await DynamicFilter(QuestionTypeDAOs, filter);
            QuestionTypeDAOs = await OrFilter(QuestionTypeDAOs, filter);
            return await QuestionTypeDAOs.CountAsync();
        }

        public async Task<List<QuestionType>> List(QuestionTypeFilter filter)
        {
            if (filter == null) return new List<QuestionType>();
            IQueryable<QuestionTypeDAO> QuestionTypeDAOs = DataContext.QuestionType.AsNoTracking();
            QuestionTypeDAOs = await DynamicFilter(QuestionTypeDAOs, filter);
            QuestionTypeDAOs = await OrFilter(QuestionTypeDAOs, filter);
            QuestionTypeDAOs = DynamicOrder(QuestionTypeDAOs, filter);
            List<QuestionType> QuestionTypes = await DynamicSelect(QuestionTypeDAOs, filter);
            return QuestionTypes;
        }

        public async Task<List<QuestionType>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<QuestionTypeDAO> query = DataContext.QuestionType.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<QuestionType> QuestionTypes = await query.AsNoTracking()
            .Select(x => new QuestionType()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToListAsync();
            

            return QuestionTypes;
        }

    }
}
