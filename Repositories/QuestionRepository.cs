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
    public interface IQuestionRepository
    {
        Task<int> CountAll(QuestionFilter QuestionFilter);
        Task<int> Count(QuestionFilter QuestionFilter);
        Task<List<Question>> List(QuestionFilter QuestionFilter);
        Task<List<Question>> List(List<long> Ids);
        Task<Question> Get(long Id);
        Task<bool> Create(Question Question);
        Task<bool> Update(Question Question);
        Task<bool> Delete(Question Question);
        Task<List<long>> BulkMerge(List<Question> Questions);
        Task<bool> BulkDelete(List<Question> Questions);
    }
    public class QuestionRepository : IQuestionRepository
    {
        private DataContext DataContext;
        public QuestionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<QuestionDAO>> DynamicFilter(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Content, filter.Content);
            query = query.Where(q => q.GradeId, filter.GradeId);
            query = query.Where(q => q.QuestionGroupId, filter.QuestionGroupId);
            query = query.Where(q => q.QuestionTypeId, filter.QuestionTypeId);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.SubjectId, filter.SubjectId);
            if (filter.Search != null)
            {
                 //query = query.Where(q => 
                 //   (filter.SearchBy.Contains(QuestionSearch.Code) && q.Code.ToLower().Contains(filter.Search.ToLower())) ||
                 //   (filter.SearchBy.Contains(QuestionSearch.Name) && q.Name.ToLower().Contains(filter.Search.ToLower())) ||
                 //   (filter.SearchBy.Contains(QuestionSearch.Content) && q.Content.ToLower().Contains(filter.Search.ToLower()))
                 //   );

                query = query.Where(q => q.Code.ToLower().Contains(filter.Search.ToLower()) ||
                    q.Name.ToLower().Contains(filter.Search.ToLower()) ||
                    q.Content.ToLower().Contains(filter.Search.ToLower())
                );
            }

            return query;
        }

        private async Task<IQueryable<QuestionDAO>> OrFilter(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<QuestionDAO> initQuery = query.Where(q => false);
            foreach (QuestionFilter QuestionFilter in filter.OrFilter)
            {
                IQueryable<QuestionDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, QuestionFilter.Id);
                queryable = queryable.Where(q => q.Code, QuestionFilter.Code);
                queryable = queryable.Where(q => q.Name, QuestionFilter.Name);
                queryable = queryable.Where(q => q.Content, QuestionFilter.Content);
                queryable = queryable.Where(q => q.GradeId, QuestionFilter.GradeId);
                queryable = queryable.Where(q => q.QuestionGroupId, QuestionFilter.QuestionGroupId);
                queryable = queryable.Where(q => q.QuestionTypeId, QuestionFilter.QuestionTypeId);
                queryable = queryable.Where(q => q.StatusId, QuestionFilter.StatusId);
                queryable = queryable.Where(q => q.SubjectId, QuestionFilter.SubjectId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<QuestionDAO> DynamicOrder(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case QuestionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case QuestionOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case QuestionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case QuestionOrder.Subject:
                            query = query.OrderBy(q => q.SubjectId);
                            break;
                        case QuestionOrder.QuestionGroup:
                            query = query.OrderBy(q => q.QuestionGroupId);
                            break;
                        case QuestionOrder.QuestionType:
                            query = query.OrderBy(q => q.QuestionTypeId);
                            break;
                        case QuestionOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                        case QuestionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case QuestionOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case QuestionOrder.Grade:
                            query = query.OrderBy(q => q.GradeId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case QuestionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case QuestionOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case QuestionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case QuestionOrder.Subject:
                            query = query.OrderByDescending(q => q.SubjectId);
                            break;
                        case QuestionOrder.QuestionGroup:
                            query = query.OrderByDescending(q => q.QuestionGroupId);
                            break;
                        case QuestionOrder.QuestionType:
                            query = query.OrderByDescending(q => q.QuestionTypeId);
                            break;
                        case QuestionOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case QuestionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case QuestionOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case QuestionOrder.Grade:
                            query = query.OrderByDescending(q => q.GradeId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Question>> DynamicSelect(IQueryable<QuestionDAO> query, QuestionFilter filter)
        {
            List<Question> Questions = await query.Select(q => new Question()
            {
                Id = filter.Selects.Contains(QuestionSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(QuestionSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(QuestionSelect.Name) ? q.Name : default(string),
                SubjectId = filter.Selects.Contains(QuestionSelect.Subject) ? q.SubjectId : default(long),
                QuestionGroupId = filter.Selects.Contains(QuestionSelect.QuestionGroup) ? q.QuestionGroupId : default(long),
                QuestionTypeId = filter.Selects.Contains(QuestionSelect.QuestionType) ? q.QuestionTypeId : default(long),
                Content = filter.Selects.Contains(QuestionSelect.Content) ? q.Content : default(string),
                StatusId = filter.Selects.Contains(QuestionSelect.Status) ? q.StatusId : default(long),
                CreatorId = filter.Selects.Contains(QuestionSelect.Creator) ? q.CreatorId : default(long),
                GradeId = filter.Selects.Contains(QuestionSelect.Grade) ? q.GradeId : default(long),
                Grade = filter.Selects.Contains(QuestionSelect.Grade) && q.Grade != null ? new Grade
                {
                    Id = q.Grade.Id,
                    Code = q.Grade.Code,
                    Name = q.Grade.Name,
                } : null,
                QuestionGroup = filter.Selects.Contains(QuestionSelect.QuestionGroup) && q.QuestionGroup != null ? new QuestionGroup
                {
                    Id = q.QuestionGroup.Id,
                    Code = q.QuestionGroup.Code,
                    Name = q.QuestionGroup.Name,
                } : null,
                QuestionType = filter.Selects.Contains(QuestionSelect.QuestionType) && q.QuestionType != null ? new QuestionType
                {
                    Id = q.QuestionType.Id,
                    Code = q.QuestionType.Code,
                    Name = q.QuestionType.Name,
                } : null,
                Status = filter.Selects.Contains(QuestionSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Subject = filter.Selects.Contains(QuestionSelect.Subject) && q.Subject != null ? new Subject
                {
                    Id = q.Subject.Id,
                    Code = q.Subject.Code,
                    Name = q.Subject.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Questions;
        }

        public async Task<int> CountAll(QuestionFilter filter)
        {
            IQueryable<QuestionDAO> QuestionDAOs = DataContext.Question.AsNoTracking();
            QuestionDAOs = await DynamicFilter(QuestionDAOs, filter);
            return await QuestionDAOs.CountAsync();
        }

        public async Task<int> Count(QuestionFilter filter)
        {
            IQueryable<QuestionDAO> QuestionDAOs = DataContext.Question.AsNoTracking();
            QuestionDAOs = await DynamicFilter(QuestionDAOs, filter);
            QuestionDAOs = await OrFilter(QuestionDAOs, filter);
            return await QuestionDAOs.CountAsync();
        }

        public async Task<List<Question>> List(QuestionFilter filter)
        {
            if (filter == null) return new List<Question>();
            IQueryable<QuestionDAO> QuestionDAOs = DataContext.Question.AsNoTracking();
            QuestionDAOs = await DynamicFilter(QuestionDAOs, filter);
            QuestionDAOs = await OrFilter(QuestionDAOs, filter);
            QuestionDAOs = DynamicOrder(QuestionDAOs, filter);
            List<Question> Questions = await DynamicSelect(QuestionDAOs, filter);
            return Questions;
        }

        public async Task<List<Question>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<QuestionDAO> query = DataContext.Question.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Question> Questions = await query.AsNoTracking()
            .Select(x => new Question()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SubjectId = x.SubjectId,
                QuestionGroupId = x.QuestionGroupId,
                QuestionTypeId = x.QuestionTypeId,
                Content = x.Content,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                GradeId = x.GradeId,
                Grade = x.Grade == null ? null : new Grade
                {
                    Id = x.Grade.Id,
                    Code = x.Grade.Code,
                    Name = x.Grade.Name,
                },
                QuestionGroup = x.QuestionGroup == null ? null : new QuestionGroup
                {
                    Id = x.QuestionGroup.Id,
                    Code = x.QuestionGroup.Code,
                    Name = x.QuestionGroup.Name,
                },
                QuestionType = x.QuestionType == null ? null : new QuestionType
                {
                    Id = x.QuestionType.Id,
                    Code = x.QuestionType.Code,
                    Name = x.QuestionType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Subject = x.Subject == null ? null : new Subject
                {
                    Id = x.Subject.Id,
                    Code = x.Subject.Code,
                    Name = x.Subject.Name,
                },
            }).ToListAsync();
            
            var QuestionContentQuery = DataContext.QuestionContent.AsNoTracking()
                .Where(x => x.QuestionId, IdFilter);
            List<QuestionContent> QuestionContents = await QuestionContentQuery
                .Select(x => new QuestionContent
                {
                    Id = x.Id,
                    QuestionId = x.QuestionId,
                    AnswerContent = x.AnswerContent,
                    IsRight = x.IsRight,
                }).ToListAsync();

            foreach(Question Question in Questions)
            {
                Question.QuestionContents = QuestionContents
                    .Where(x => x.QuestionId == Question.Id)
                    .ToList();
            }


            return Questions;
        }

        public async Task<Question> Get(long Id)
        {
            Question Question = await DataContext.Question.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Question()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SubjectId = x.SubjectId,
                QuestionGroupId = x.QuestionGroupId,
                QuestionTypeId = x.QuestionTypeId,
                Content = x.Content,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                GradeId = x.GradeId,
                Grade = x.Grade == null ? null : new Grade
                {
                    Id = x.Grade.Id,
                    Code = x.Grade.Code,
                    Name = x.Grade.Name,
                },
                QuestionGroup = x.QuestionGroup == null ? null : new QuestionGroup
                {
                    Id = x.QuestionGroup.Id,
                    Code = x.QuestionGroup.Code,
                    Name = x.QuestionGroup.Name,
                },
                QuestionType = x.QuestionType == null ? null : new QuestionType
                {
                    Id = x.QuestionType.Id,
                    Code = x.QuestionType.Code,
                    Name = x.QuestionType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
                Subject = x.Subject == null ? null : new Subject
                {
                    Id = x.Subject.Id,
                    Code = x.Subject.Code,
                    Name = x.Subject.Name,
                },
            }).FirstOrDefaultAsync();

            if (Question == null)
                return null;
            Question.QuestionContents = await DataContext.QuestionContent.AsNoTracking()
                .Where(x => x.QuestionId == Question.Id)
                .Select(x => new QuestionContent
                {
                    Id = x.Id,
                    QuestionId = x.QuestionId,
                    AnswerContent = x.AnswerContent,
                    IsRight = x.IsRight,
                }).ToListAsync();

            return Question;
        }
        public async Task<bool> Create(Question Question)
        {
            QuestionDAO QuestionDAO = new QuestionDAO();
            QuestionDAO.Id = Question.Id;
            QuestionDAO.Code = Question.Code;
            QuestionDAO.Name = Question.Name;
            QuestionDAO.SubjectId = Question.SubjectId;
            QuestionDAO.QuestionGroupId = Question.QuestionGroupId;
            QuestionDAO.QuestionTypeId = Question.QuestionTypeId;
            QuestionDAO.Content = Question.Content;
            QuestionDAO.StatusId = Question.StatusId;
            QuestionDAO.CreatorId = Question.CreatorId;
            QuestionDAO.GradeId = Question.GradeId;
            QuestionDAO.RowId = Guid.NewGuid();
            QuestionDAO.CreatedAt = StaticParams.DateTimeNow;
            QuestionDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Question.Add(QuestionDAO);
            await DataContext.SaveChangesAsync();
            Question.Id = QuestionDAO.Id;
            await SaveReference(Question);
            return true;
        }

        public async Task<bool> Update(Question Question)
        {
            QuestionDAO QuestionDAO = DataContext.Question
                .Where(x => x.Id == Question.Id)
                .FirstOrDefault();
            if (QuestionDAO == null)
                return false;
            QuestionDAO.Id = Question.Id;
            QuestionDAO.Code = Question.Code;
            QuestionDAO.Name = Question.Name;
            QuestionDAO.SubjectId = Question.SubjectId;
            QuestionDAO.QuestionGroupId = Question.QuestionGroupId;
            QuestionDAO.QuestionTypeId = Question.QuestionTypeId;
            QuestionDAO.Content = Question.Content;
            QuestionDAO.StatusId = Question.StatusId;
            QuestionDAO.CreatorId = Question.CreatorId;
            QuestionDAO.GradeId = Question.GradeId;
            QuestionDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Question);
            return true;
        }

        public async Task<bool> Delete(Question Question)
        {
            await DataContext.Question
                .Where(x => x.Id == Question.Id)
                .UpdateFromQueryAsync(x => new QuestionDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        public async Task<List<long>> BulkMerge(List<Question> Questions)
        {
            IdFilter IdFilter = new IdFilter { In = Questions.Select(x => x.Id).ToList() };
            List<QuestionDAO> Inserts = new List<QuestionDAO>();
            List<QuestionDAO> Updates = new List<QuestionDAO>();
            List<QuestionDAO> DbQuestionDAOs = await DataContext.Question
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Question Question in Questions)
            {
                QuestionDAO QuestionDAO = DbQuestionDAOs
                        .Where(x => x.Id == Question.Id)
                        .FirstOrDefault();
                if (QuestionDAO == null)
                {
                    QuestionDAO = new QuestionDAO();
                    QuestionDAO.CreatedAt = StaticParams.DateTimeNow;
                    QuestionDAO.RowId = Guid.NewGuid();
                    QuestionDAO.Id = Question.Id;
                    Question.RowId = QuestionDAO.RowId;
                    Inserts.Add(QuestionDAO);
                }
                else
                {
                    Updates.Add(QuestionDAO);
                }
                QuestionDAO.Code = Question.Code;
                QuestionDAO.Name = Question.Name;
                QuestionDAO.SubjectId = Question.SubjectId;
                QuestionDAO.QuestionGroupId = Question.QuestionGroupId;
                QuestionDAO.QuestionTypeId = Question.QuestionTypeId;
                QuestionDAO.Content = Question.Content;
                QuestionDAO.StatusId = Question.StatusId;
                QuestionDAO.CreatorId = Question.CreatorId;
                QuestionDAO.GradeId = Question.GradeId;
                QuestionDAO.UpdatedAt = StaticParams.DateTimeNow;
            }
            await DataContext.Question.BulkInsertAsync(Inserts);
            await DataContext.Question.BulkMergeAsync(Updates);
            var Ids = Inserts.Select(x => x.Id).ToList();
            Ids.AddRange(Updates.Select(x => x.Id));
            Ids = Ids.Distinct().ToList();
            return Ids;
        }
        
        public async Task<bool> BulkDelete(List<Question> Questions)
        {
            List<long> Ids = Questions.Select(x => x.Id).ToList();
            await DataContext.Question
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new QuestionDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow,
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Question Question)
        {
            await DataContext.QuestionContent
                .Where(x => x.QuestionId == Question.Id)
                .DeleteFromQueryAsync();
            if (Question.QuestionContents != null)
            {
                List<QuestionContentDAO> QuestionContentDAOs = new List<QuestionContentDAO>();
                foreach (QuestionContent QuestionContent in Question.QuestionContents)
                {
                    QuestionContentDAO QuestionContentDAO = new QuestionContentDAO();
                    QuestionContentDAO.Id = QuestionContent.Id;
                    QuestionContentDAO.QuestionId = Question.Id;
                    QuestionContentDAO.AnswerContent = QuestionContent.AnswerContent;
                    QuestionContentDAO.IsRight = QuestionContent.IsRight;
                    QuestionContentDAOs.Add(QuestionContentDAO);
                }
                await DataContext.BulkMergeAsync(QuestionContentDAOs);
            }
        }

    }
}
