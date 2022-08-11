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
    public interface IExamRepository
    {
        Task<int> CountAll(ExamFilter ExamFilter);
        Task<int> Count(ExamFilter ExamFilter);
        Task<List<Exam>> List(ExamFilter ExamFilter);
        Task<List<Exam>> List(List<long> Ids);
        Task<Exam> Get(long Id);
        Task<bool> Create(Exam Exam);
        Task<bool> Update(Exam Exam);
        Task<bool> Delete(Exam Exam);
        Task<List<long>> BulkMerge(List<Exam> Exams);
        Task<bool> BulkDelete(List<Exam> Exams);
    }
    public class ExamRepository : IExamRepository
    {
        private DataContext DataContext;
        public ExamRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ExamDAO>> DynamicFilter(IQueryable<ExamDAO> query, ExamFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Code, filter.Code);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.TotalMark, filter.TotalMark);
            query = query.Where(q => q.TotalQuestion, filter.TotalQuestion);
            query = query.Where(q => q.Time, filter.Time);
            query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = query.Where(q => q.ExamLevelId, filter.ExamLevelId);
            query = query.Where(q => q.ExamStatusId, filter.ExamStatusId);
            query = query.Where(q => q.GradeId, filter.GradeId);
            query = query.Where(q => q.ImageId, filter.ImageId);
            query = query.Where(q => q.StatusId, filter.StatusId);
            query = query.Where(q => q.SubjectId, filter.SubjectId);
            if (filter.Search != null)
            {
                List<long> ExamIds = new List<long>();
                if (filter.SearchBy.Contains(ExamSearch.Question))
                {
                    ExamIds = await DataContext.ExamQuestionMapping.AsNoTracking()
                    .Where(q => q.Question.Content.ToLower().Contains(filter.Search.ToLower())).
                    Select(x => x.ExamId)
                    .ToListAsync();

                    //if (ExamIds != null)
                    //{
                    //    ExamIds = ExamIds.Distinct().ToList();
                    //    IdFilter IdFilter = new IdFilter { In = ExamIds };
                    //    query = query.Where(q => q.Id, IdFilter);       
                    //}

                }
               
                if (ExamIds.Count() > 0)
                {
                     query = query.Where(q =>
                     q.Code.ToLower().Contains(filter.Search.ToLower()) ||
                     q.Name.ToLower().Contains(filter.Search.ToLower()) ||
                     ExamIds.Contains(q.Id)
                    );
                }
                else
                {
                  query = query.Where(q =>
                  q.Code.ToLower().Contains(filter.Search.ToLower()) ||
                  q.Name.ToLower().Contains(filter.Search.ToLower())
               );
                }

            }

            return query;
        }

        private async Task<IQueryable<ExamDAO>> OrFilter(IQueryable<ExamDAO> query, ExamFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ExamDAO> initQuery = query.Where(q => false);
            foreach (ExamFilter ExamFilter in filter.OrFilter)
            {
                IQueryable<ExamDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ExamFilter.Id);
                queryable = queryable.Where(q => q.Code, ExamFilter.Code);
                queryable = queryable.Where(q => q.Name, ExamFilter.Name);
                queryable = queryable.Where(q => q.TotalMark, ExamFilter.TotalMark);
                queryable = queryable.Where(q => q.TotalQuestion, ExamFilter.TotalQuestion);
                queryable = queryable.Where(q => q.Time, ExamFilter.Time);
                queryable = queryable.Where(q => q.CreatorId, ExamFilter.CreatorId);
                queryable = queryable.Where(q => q.ExamLevelId, ExamFilter.ExamLevelId);
                queryable = queryable.Where(q => q.ExamStatusId, ExamFilter.ExamStatusId);
                queryable = queryable.Where(q => q.GradeId, ExamFilter.GradeId);
                queryable = queryable.Where(q => q.ImageId, ExamFilter.ImageId);
                queryable = queryable.Where(q => q.StatusId, ExamFilter.StatusId);
                queryable = queryable.Where(q => q.SubjectId, ExamFilter.SubjectId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ExamDAO> DynamicOrder(IQueryable<ExamDAO> query, ExamFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ExamOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ExamOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ExamOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ExamOrder.Subject:
                            query = query.OrderBy(q => q.SubjectId);
                            break;
                        case ExamOrder.ExamLevel:
                            query = query.OrderBy(q => q.ExamLevelId);
                            break;
                        case ExamOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ExamOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case ExamOrder.Grade:
                            query = query.OrderBy(q => q.GradeId);
                            break;
                        case ExamOrder.ExamStatus:
                            query = query.OrderBy(q => q.ExamStatusId);
                            break;
                        case ExamOrder.TotalMark:
                            query = query.OrderBy(q => q.TotalMark);
                            break;
                        case ExamOrder.TotalQuestion:
                            query = query.OrderBy(q => q.TotalQuestion);
                            break;
                        case ExamOrder.Image:
                            query = query.OrderBy(q => q.ImageId);
                            break;
                        case ExamOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ExamOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ExamOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ExamOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ExamOrder.Subject:
                            query = query.OrderByDescending(q => q.SubjectId);
                            break;
                        case ExamOrder.ExamLevel:
                            query = query.OrderByDescending(q => q.ExamLevelId);
                            break;
                        case ExamOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ExamOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case ExamOrder.Grade:
                            query = query.OrderByDescending(q => q.GradeId);
                            break;
                        case ExamOrder.ExamStatus:
                            query = query.OrderByDescending(q => q.ExamStatusId);
                            break;
                        case ExamOrder.TotalMark:
                            query = query.OrderByDescending(q => q.TotalMark);
                            break;
                        case ExamOrder.TotalQuestion:
                            query = query.OrderByDescending(q => q.TotalQuestion);
                            break;
                        case ExamOrder.Image:
                            query = query.OrderByDescending(q => q.ImageId);
                            break;
                        case ExamOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Exam>> DynamicSelect(IQueryable<ExamDAO> query, ExamFilter filter)
        {
            List<Exam> Exams = await query.Select(q => new Exam()
            {
                Id = filter.Selects.Contains(ExamSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ExamSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ExamSelect.Name) ? q.Name : default(string),
                SubjectId = filter.Selects.Contains(ExamSelect.Subject) ? q.SubjectId : default(long),
                ExamLevelId = filter.Selects.Contains(ExamSelect.ExamLevel) ? q.ExamLevelId : default(long),
                StatusId = filter.Selects.Contains(ExamSelect.Status) ? q.StatusId : default(long),
                CreatorId = filter.Selects.Contains(ExamSelect.Creator) ? q.CreatorId : default(long),
                GradeId = filter.Selects.Contains(ExamSelect.Grade) ? q.GradeId : default(long),
                ExamStatusId = filter.Selects.Contains(ExamSelect.ExamStatus) ? q.ExamStatusId : default(long),
                TotalMark = filter.Selects.Contains(ExamSelect.TotalMark) ? q.TotalMark : default(decimal?),
                TotalQuestion = filter.Selects.Contains(ExamSelect.TotalQuestion) ? q.TotalQuestion : default(long),
                ImageId = filter.Selects.Contains(ExamSelect.Image) ? q.ImageId : default(long?),
                Time = filter.Selects.Contains(ExamSelect.Time) ? q.Time : default(long?),
                CurrentMonthNumberTest = filter.Selects.Contains(ExamSelect.CurrentMonthNumberTest) ? q.CurrentMonthNumberTest : default(long),
                TotalNumberTest = filter.Selects.Contains(ExamSelect.TotalNumberTest) ? q.TotalNumberTest : default(long),

                Creator = filter.Selects.Contains(ExamSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    ImageId = q.Creator.ImageId,
                } : null,
                ExamLevel = filter.Selects.Contains(ExamSelect.ExamLevel) && q.ExamLevel != null ? new ExamLevel
                {
                    Id = q.ExamLevel.Id,
                    Code = q.ExamLevel.Code,
                    Name = q.ExamLevel.Name,
                } : null,
                ExamStatus = filter.Selects.Contains(ExamSelect.ExamStatus) && q.ExamStatus != null ? new ExamStatus
                {
                    Id = q.ExamStatus.Id,
                    Code = q.ExamStatus.Code,
                    Name = q.ExamStatus.Name,
                } : null,
                Grade = filter.Selects.Contains(ExamSelect.Grade) && q.Grade != null ? new Grade
                {
                    Id = q.Grade.Id,
                    Code = q.Grade.Code,
                    Name = q.Grade.Name,
                } : null,
                Image = filter.Selects.Contains(ExamSelect.Image) && q.Image != null ? new Image
                {
                    Id = q.Image.Id,
                    Name = q.Image.Name,
                    Url = q.Image.Url,
                } : null,
                Status = filter.Selects.Contains(ExamSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Subject = filter.Selects.Contains(ExamSelect.Subject) && q.Subject != null ? new Subject
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
            return Exams;
        }

        public async Task<int> CountAll(ExamFilter filter)
        {
            IQueryable<ExamDAO> ExamDAOs = DataContext.Exam.AsNoTracking();
            ExamDAOs = await DynamicFilter(ExamDAOs, filter);
            return await ExamDAOs.CountAsync();
        }

        public async Task<int> Count(ExamFilter filter)
        {
            IQueryable<ExamDAO> ExamDAOs = DataContext.Exam.AsNoTracking();
            ExamDAOs = await DynamicFilter(ExamDAOs, filter);
            ExamDAOs = await OrFilter(ExamDAOs, filter);
            return await ExamDAOs.CountAsync();
        }

        public async Task<List<Exam>> List(ExamFilter filter)
        {
            if (filter == null) return new List<Exam>();
            IQueryable<ExamDAO> ExamDAOs = DataContext.Exam.AsNoTracking();
            ExamDAOs = await DynamicFilter(ExamDAOs, filter);
            ExamDAOs = await OrFilter(ExamDAOs, filter);
            ExamDAOs = DynamicOrder(ExamDAOs, filter);
            List<Exam> Exams = await DynamicSelect(ExamDAOs, filter);
            return Exams;
        }

        public async Task<List<Exam>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ExamDAO> query = DataContext.Exam.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Exam> Exams = await query.AsNoTracking()
            .Select(x => new Exam()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SubjectId = x.SubjectId,
                ExamLevelId = x.ExamLevelId,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                GradeId = x.GradeId,
                ExamStatusId = x.ExamStatusId,
                TotalMark = x.TotalMark,
                TotalQuestion = x.TotalQuestion,
                ImageId = x.ImageId,
                Time = x.Time,
                CurrentMonthNumberTest = x.CurrentMonthNumberTest,
                TotalNumberTest = x.TotalNumberTest,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    ImageId = x.Creator.ImageId,
                },
                ExamLevel = x.ExamLevel == null ? null : new ExamLevel
                {
                    Id = x.ExamLevel.Id,
                    Code = x.ExamLevel.Code,
                    Name = x.ExamLevel.Name,
                },
                ExamStatus = x.ExamStatus == null ? null : new ExamStatus
                {
                    Id = x.ExamStatus.Id,
                    Code = x.ExamStatus.Code,
                    Name = x.ExamStatus.Name,
                },
                Grade = x.Grade == null ? null : new Grade
                {
                    Id = x.Grade.Id,
                    Code = x.Grade.Code,
                    Name = x.Grade.Name,
                },
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
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
            
            var ExamQuestionMappingQuery = DataContext.ExamQuestionMapping.AsNoTracking()
                .Where(x => x.ExamId, IdFilter);
            List<ExamQuestionMapping> ExamQuestionMappings = await ExamQuestionMappingQuery
                .Where(x => x.Question.DeletedAt == null)
                .Select(x => new ExamQuestionMapping
                {
                    ExamId = x.ExamId,
                    QuestionId = x.QuestionId,
                    Mark = x.Mark,
                    Question = new Question
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

            foreach (Exam Exam in Exams)
            {
                Exam.ExamQuestionMappings = ExamQuestionMappings
                    .Where(x => x.ExamId == Exam.Id)
                    .ToList();
            }

            return Exams;
        }

        public async Task<Exam> Get(long Id)
        {
            Exam Exam = await DataContext.Exam.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Exam()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SubjectId = x.SubjectId,
                ExamLevelId = x.ExamLevelId,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                GradeId = x.GradeId,
                ExamStatusId = x.ExamStatusId,
                TotalMark = x.TotalMark,
                TotalQuestion = x.TotalQuestion,
                ImageId = x.ImageId,
                Time = x.Time,
                CurrentMonthNumberTest = x.CurrentMonthNumberTest,
                TotalNumberTest = x.TotalNumberTest,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    ImageId = x.Creator.ImageId,
                },
                ExamLevel = x.ExamLevel == null ? null : new ExamLevel
                {
                    Id = x.ExamLevel.Id,
                    Code = x.ExamLevel.Code,
                    Name = x.ExamLevel.Name,
                },
                ExamStatus = x.ExamStatus == null ? null : new ExamStatus
                {
                    Id = x.ExamStatus.Id,
                    Code = x.ExamStatus.Code,
                    Name = x.ExamStatus.Name,
                },
                Grade = x.Grade == null ? null : new Grade
                {
                    Id = x.Grade.Id,
                    Code = x.Grade.Code,
                    Name = x.Grade.Name,
                },
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
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

            if (Exam == null)
                return null;
            Exam.ExamQuestionMappings = await DataContext.ExamQuestionMapping.AsNoTracking()
                .Where(x => x.ExamId == Exam.Id)
                .Where(x => x.Question.DeletedAt == null)
                .Select(x => new ExamQuestionMapping
                {
                    ExamId = x.ExamId,
                    QuestionId = x.QuestionId,
                    Mark = x.Mark,
                    Question = new Question
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

            var QuestionIds = Exam.ExamQuestionMappings.Select(x => x.QuestionId).Distinct().ToList();
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext.BulkInsertValuesIntoTempTableAsync<long>(QuestionIds);

            var queryQC = from x in DataContext.QuestionContent
                          join tt in tempTableQuery.Query on x.QuestionId equals tt.Column1
                          select x;
            List<QuestionContent> QuestionContents = await queryQC.
                Select(x => new QuestionContent
                {
                    Id = x.Id,
                    QuestionId = x.QuestionId,
                    AnswerContent = x.AnswerContent,
                    IsRight = x.IsRight
                }).ToListAsync();
            if (Exam.ExamQuestionMappings?.Any() ?? false)
            {
                foreach (var ExamQuestionMapping in Exam.ExamQuestionMappings)
                {
                    ExamQuestionMapping.Question.QuestionContents = QuestionContents
                        .Where(x => x.QuestionId == ExamQuestionMapping.QuestionId).ToList();
                }
            }
            return Exam;
        }
        public async Task<bool> Create(Exam Exam)
        {
            ExamDAO ExamDAO = new ExamDAO();
            ExamDAO.Id = Exam.Id;
            ExamDAO.Code = Exam.Code;
            ExamDAO.Name = Exam.Name;
            ExamDAO.SubjectId = Exam.SubjectId;
            ExamDAO.ExamLevelId = Exam.ExamLevelId;
            ExamDAO.StatusId = Exam.StatusId;
            ExamDAO.CreatorId = Exam.CreatorId;
            ExamDAO.GradeId = Exam.GradeId;
            ExamDAO.ExamStatusId = Exam.ExamStatusId;
            ExamDAO.TotalMark = Exam.TotalMark;
            ExamDAO.TotalQuestion = Exam.TotalQuestion;
            ExamDAO.ImageId = Exam.ImageId;
            ExamDAO.Time = Exam.Time;
            ExamDAO.CurrentMonthNumberTest = Exam.CurrentMonthNumberTest;
            ExamDAO.TotalNumberTest = Exam.TotalNumberTest;
            ExamDAO.RowId = Guid.NewGuid();
            ExamDAO.CreatedAt = StaticParams.DateTimeNow;
            ExamDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Exam.Add(ExamDAO);
            await DataContext.SaveChangesAsync();
            Exam.Id = ExamDAO.Id;
            await SaveReference(Exam);
            return true;
        }

        public async Task<bool> Update(Exam Exam)
        {
            ExamDAO ExamDAO = DataContext.Exam
                .Where(x => x.Id == Exam.Id)
                .FirstOrDefault();
            if (ExamDAO == null)
                return false;
            ExamDAO.Id = Exam.Id;
            ExamDAO.Code = Exam.Code;
            ExamDAO.Name = Exam.Name;
            ExamDAO.SubjectId = Exam.SubjectId;
            ExamDAO.ExamLevelId = Exam.ExamLevelId;
            ExamDAO.StatusId = Exam.StatusId;
            ExamDAO.CreatorId = Exam.CreatorId;
            ExamDAO.GradeId = Exam.GradeId;
            ExamDAO.ExamStatusId = Exam.ExamStatusId;
            ExamDAO.TotalMark = Exam.TotalMark;
            ExamDAO.TotalQuestion = Exam.TotalQuestion;
            ExamDAO.ImageId = Exam.ImageId;
            ExamDAO.Time = Exam.Time;
            ExamDAO.CurrentMonthNumberTest = Exam.CurrentMonthNumberTest;
            ExamDAO.TotalNumberTest = Exam.TotalNumberTest;
            ExamDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Exam);
            return true;
        }

        public async Task<bool> Delete(Exam Exam)
        {
            await DataContext.Exam
                .Where(x => x.Id == Exam.Id)
                .UpdateFromQueryAsync(x => new ExamDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        public async Task<List<long>> BulkMerge(List<Exam> Exams)
        {
            IdFilter IdFilter = new IdFilter { In = Exams.Select(x => x.Id).ToList() };
            List<ExamDAO> Inserts = new List<ExamDAO>();
            List<ExamDAO> Updates = new List<ExamDAO>();
            List<ExamDAO> DbExamDAOs = await DataContext.Exam
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Exam Exam in Exams)
            {
                ExamDAO ExamDAO = DbExamDAOs
                        .Where(x => x.Id == Exam.Id)
                        .FirstOrDefault();
                if (ExamDAO == null)
                {
                    ExamDAO = new ExamDAO();
                    ExamDAO.CreatedAt = StaticParams.DateTimeNow;
                    ExamDAO.RowId = Guid.NewGuid();
                    ExamDAO.Id = Exam.Id;
                    Exam.RowId = ExamDAO.RowId;
                    Inserts.Add(ExamDAO);
                }
                else
                {
                    Updates.Add(ExamDAO);
                }
                ExamDAO.Code = Exam.Code;
                ExamDAO.Name = Exam.Name;
                ExamDAO.SubjectId = Exam.SubjectId;
                ExamDAO.ExamLevelId = Exam.ExamLevelId;
                ExamDAO.StatusId = Exam.StatusId;
                ExamDAO.CreatorId = Exam.CreatorId;
                ExamDAO.GradeId = Exam.GradeId;
                ExamDAO.ExamStatusId = Exam.ExamStatusId;
                ExamDAO.TotalMark = Exam.TotalMark;
                ExamDAO.TotalQuestion = Exam.TotalQuestion;
                ExamDAO.ImageId = Exam.ImageId;
                ExamDAO.Time = Exam.Time;
                ExamDAO.UpdatedAt = StaticParams.DateTimeNow;
            }
            await DataContext.Exam.BulkInsertAsync(Inserts);
            await DataContext.Exam.BulkMergeAsync(Updates);
            var Ids = Inserts.Select(x => x.Id).ToList();
            Ids.AddRange(Updates.Select(x => x.Id));
            Ids = Ids.Distinct().ToList();
            return Ids;
        }
        
        public async Task<bool> BulkDelete(List<Exam> Exams)
        {
            List<long> Ids = Exams.Select(x => x.Id).ToList();
            await DataContext.Exam
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new ExamDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Exam Exam)
        {
            await DataContext.ExamQuestionMapping
                .Where(x => x.ExamId == Exam.Id)
                .DeleteFromQueryAsync();

            List<ExamQuestionMappingDAO> ExamQuestionMappingDAOs = new List<ExamQuestionMappingDAO>();
            if (Exam.ExamQuestionMappings != null)
            {
                foreach (ExamQuestionMapping ExamQuestionMapping in Exam.ExamQuestionMappings)
                {
                    ExamQuestionMappingDAO ExamQuestionMappingDAO = new ExamQuestionMappingDAO();
                    ExamQuestionMappingDAO.ExamId = Exam.Id;
                    ExamQuestionMappingDAO.QuestionId = ExamQuestionMapping.QuestionId;
                    ExamQuestionMappingDAO.Mark = ExamQuestionMapping.Mark;
                    ExamQuestionMappingDAOs.Add(ExamQuestionMappingDAO);
                }
                await DataContext.ExamQuestionMapping.BulkInsertAsync(ExamQuestionMappingDAOs);
            }
        }

    }
}
