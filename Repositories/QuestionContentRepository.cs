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
    public interface IQuestionContentRepository
    {
        Task<int> CountAll(QuestionContentFilter QuestionContentFilter);
        Task<int> Count(QuestionContentFilter QuestionContentFilter);
        Task<List<QuestionContent>> List(QuestionContentFilter QuestionContentFilter);
        Task<List<QuestionContent>> List(List<long> Ids);
        Task<QuestionContent> Get(long Id);
        Task<bool> Create(QuestionContent QuestionContent);
        Task<bool> Update(QuestionContent QuestionContent);
        Task<bool> Delete(QuestionContent QuestionContent);
        Task<List<long>> BulkMerge(List<QuestionContent> QuestionContents);
        Task<bool> BulkDelete(List<QuestionContent> QuestionContents);
    }
    public class QuestionContentRepository : IQuestionContentRepository
    {
        private DataContext DataContext;
        public QuestionContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<QuestionContentDAO>> DynamicFilter(IQueryable<QuestionContentDAO> query, QuestionContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.AnswerContent, filter.AnswerContent);
            query = query.Where(q => q.IsRight, filter.IsRight);
            query = query.Where(q => q.QuestionId, filter.QuestionId);

            return query;
        }

        private async Task<IQueryable<QuestionContentDAO>> OrFilter(IQueryable<QuestionContentDAO> query, QuestionContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<QuestionContentDAO> initQuery = query.Where(q => false);
            foreach (QuestionContentFilter QuestionContentFilter in filter.OrFilter)
            {
                IQueryable<QuestionContentDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, QuestionContentFilter.Id);
                queryable = queryable.Where(q => q.AnswerContent, QuestionContentFilter.AnswerContent);
                queryable = queryable.Where(q => q.IsRight, QuestionContentFilter.IsRight);
                queryable = queryable.Where(q => q.QuestionId, QuestionContentFilter.QuestionId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<QuestionContentDAO> DynamicOrder(IQueryable<QuestionContentDAO> query, QuestionContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case QuestionContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case QuestionContentOrder.Question:
                            query = query.OrderBy(q => q.QuestionId);
                            break;
                        case QuestionContentOrder.AnswerContent:
                            query = query.OrderBy(q => q.AnswerContent);
                            break;
                        case QuestionContentOrder.IsRight:
                            query = query.OrderBy(q => q.IsRight);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case QuestionContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case QuestionContentOrder.Question:
                            query = query.OrderByDescending(q => q.QuestionId);
                            break;
                        case QuestionContentOrder.AnswerContent:
                            query = query.OrderByDescending(q => q.AnswerContent);
                            break;
                        case QuestionContentOrder.IsRight:
                            query = query.OrderByDescending(q => q.IsRight);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<QuestionContent>> DynamicSelect(IQueryable<QuestionContentDAO> query, QuestionContentFilter filter)
        {
            List<QuestionContent> QuestionContents = await query.Select(q => new QuestionContent()
            {
                Id = filter.Selects.Contains(QuestionContentSelect.Id) ? q.Id : default(long),
                QuestionId = filter.Selects.Contains(QuestionContentSelect.Question) ? q.QuestionId : default(long),
                AnswerContent = filter.Selects.Contains(QuestionContentSelect.AnswerContent) ? q.AnswerContent : default(string),
                IsRight = filter.Selects.Contains(QuestionContentSelect.IsRight) ? q.IsRight : default(bool),
                Question = filter.Selects.Contains(QuestionContentSelect.Question) && q.Question != null ? new Question
                {
                    Id = q.Question.Id,
                    Code = q.Question.Code,
                    Name = q.Question.Name,
                    SubjectId = q.Question.SubjectId,
                    QuestionGroupId = q.Question.QuestionGroupId,
                    QuestionTypeId = q.Question.QuestionTypeId,
                    Content = q.Question.Content,
                    StatusId = q.Question.StatusId,
                    CreatorId = q.Question.CreatorId,
                    GradeId = q.Question.GradeId,
                } : null,
            }).ToListAsync();
            return QuestionContents;
        }

        public async Task<int> CountAll(QuestionContentFilter filter)
        {
            IQueryable<QuestionContentDAO> QuestionContentDAOs = DataContext.QuestionContent.AsNoTracking();
            QuestionContentDAOs = await DynamicFilter(QuestionContentDAOs, filter);
            return await QuestionContentDAOs.CountAsync();
        }

        public async Task<int> Count(QuestionContentFilter filter)
        {
            IQueryable<QuestionContentDAO> QuestionContentDAOs = DataContext.QuestionContent.AsNoTracking();
            QuestionContentDAOs = await DynamicFilter(QuestionContentDAOs, filter);
            QuestionContentDAOs = await OrFilter(QuestionContentDAOs, filter);
            return await QuestionContentDAOs.CountAsync();
        }

        public async Task<List<QuestionContent>> List(QuestionContentFilter filter)
        {
            if (filter == null) return new List<QuestionContent>();
            IQueryable<QuestionContentDAO> QuestionContentDAOs = DataContext.QuestionContent.AsNoTracking();
            QuestionContentDAOs = await DynamicFilter(QuestionContentDAOs, filter);
            QuestionContentDAOs = await OrFilter(QuestionContentDAOs, filter);
            QuestionContentDAOs = DynamicOrder(QuestionContentDAOs, filter);
            List<QuestionContent> QuestionContents = await DynamicSelect(QuestionContentDAOs, filter);
            return QuestionContents;
        }

        public async Task<List<QuestionContent>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<QuestionContentDAO> query = DataContext.QuestionContent.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<QuestionContent> QuestionContents = await query.AsNoTracking()
            .Select(x => new QuestionContent()
            {
                Id = x.Id,
                QuestionId = x.QuestionId,
                AnswerContent = x.AnswerContent,
                IsRight = x.IsRight,
                Question = x.Question == null ? null : new Question
                {
                    Id = x.Question.Id,
                    Code = x.Question.Code,
                    Name = x.Question.Name,
                    SubjectId = x.Question.SubjectId,
                    QuestionGroupId = x.Question.QuestionGroupId,
                    QuestionTypeId = x.Question.QuestionTypeId,
                    Content = x.Question.Content,
                    StatusId = x.Question.StatusId,
                    CreatorId = x.Question.CreatorId,
                    GradeId = x.Question.GradeId,
                },
            }).ToListAsync();
            

            return QuestionContents;
        }

        public async Task<QuestionContent> Get(long Id)
        {
            QuestionContent QuestionContent = await DataContext.QuestionContent.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new QuestionContent()
            {
                Id = x.Id,
                QuestionId = x.QuestionId,
                AnswerContent = x.AnswerContent,
                IsRight = x.IsRight,
                Question = x.Question == null ? null : new Question
                {
                    Id = x.Question.Id,
                    Code = x.Question.Code,
                    Name = x.Question.Name,
                    SubjectId = x.Question.SubjectId,
                    QuestionGroupId = x.Question.QuestionGroupId,
                    QuestionTypeId = x.Question.QuestionTypeId,
                    Content = x.Question.Content,
                    StatusId = x.Question.StatusId,
                    CreatorId = x.Question.CreatorId,
                    GradeId = x.Question.GradeId,
                },
            }).FirstOrDefaultAsync();

            if (QuestionContent == null)
                return null;

            return QuestionContent;
        }
        public async Task<bool> Create(QuestionContent QuestionContent)
        {
            QuestionContentDAO QuestionContentDAO = new QuestionContentDAO();
            QuestionContentDAO.Id = QuestionContent.Id;
            QuestionContentDAO.QuestionId = QuestionContent.QuestionId;
            QuestionContentDAO.AnswerContent = QuestionContent.AnswerContent;
            QuestionContentDAO.IsRight = QuestionContent.IsRight;
            DataContext.QuestionContent.Add(QuestionContentDAO);
            await DataContext.SaveChangesAsync();
            QuestionContent.Id = QuestionContentDAO.Id;
            await SaveReference(QuestionContent);
            return true;
        }

        public async Task<bool> Update(QuestionContent QuestionContent)
        {
            QuestionContentDAO QuestionContentDAO = DataContext.QuestionContent
                .Where(x => x.Id == QuestionContent.Id)
                .FirstOrDefault();
            if (QuestionContentDAO == null)
                return false;
            QuestionContentDAO.Id = QuestionContent.Id;
            QuestionContentDAO.QuestionId = QuestionContent.QuestionId;
            QuestionContentDAO.AnswerContent = QuestionContent.AnswerContent;
            QuestionContentDAO.IsRight = QuestionContent.IsRight;
            await DataContext.SaveChangesAsync();
            await SaveReference(QuestionContent);
            return true;
        }

        public async Task<bool> Delete(QuestionContent QuestionContent)
        {
            await DataContext.QuestionContent
                .Where(x => x.Id == QuestionContent.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        public async Task<List<long>> BulkMerge(List<QuestionContent> QuestionContents)
        {
            IdFilter IdFilter = new IdFilter { In = QuestionContents.Select(x => x.Id).ToList() };
            List<QuestionContentDAO> Inserts = new List<QuestionContentDAO>();
            List<QuestionContentDAO> Updates = new List<QuestionContentDAO>();
            List<QuestionContentDAO> DbQuestionContentDAOs = await DataContext.QuestionContent
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (QuestionContent QuestionContent in QuestionContents)
            {
                QuestionContentDAO QuestionContentDAO = DbQuestionContentDAOs
                        .Where(x => x.Id == QuestionContent.Id)
                        .FirstOrDefault();
                if (QuestionContentDAO == null)
                {
                    QuestionContentDAO = new QuestionContentDAO();
                    Inserts.Add(QuestionContentDAO);
                }
                else
                {
                    Updates.Add(QuestionContentDAO);
                }
                QuestionContentDAO.QuestionId = QuestionContent.QuestionId;
                QuestionContentDAO.AnswerContent = QuestionContent.AnswerContent;
                QuestionContentDAO.IsRight = QuestionContent.IsRight;
            }
            await DataContext.QuestionContent.BulkInsertAsync(Inserts);
            await DataContext.QuestionContent.BulkMergeAsync(Updates);
            var Ids = Inserts.Select(x => x.Id).ToList();
            Ids.AddRange(Updates.Select(x => x.Id));
            Ids = Ids.Distinct().ToList();
            return Ids;
        }
        
        public async Task<bool> BulkDelete(List<QuestionContent> QuestionContents)
        {
            List<long> Ids = QuestionContents.Select(x => x.Id).ToList();
            await DataContext.QuestionContent
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(QuestionContent QuestionContent)
        {
        }

    }
}
