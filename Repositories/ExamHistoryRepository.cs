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
    public interface IExamHistoryRepository
    {
        Task<int> CountAll(ExamHistoryFilter ExamHistoryFilter);
        Task<int> Count(ExamHistoryFilter ExamHistoryFilter);
        Task<List<ExamHistory>> List(ExamHistoryFilter ExamHistoryFilter);
        Task<List<ExamHistory>> List(List<long> Ids);
        Task<ExamHistory> Get(long Id);
        Task<bool> Create(ExamHistory ExamHistory);
        Task<bool> Update(ExamHistory ExamHistory);
        Task<bool> Delete(ExamHistory ExamHistory);
        Task<List<long>> BulkMerge(List<ExamHistory> ExamHistories);
        Task<bool> BulkDelete(List<ExamHistory> ExamHistories);

    }
    public class ExamHistoryRepository : IExamHistoryRepository
    {
        private DataContext DataContext;
        public ExamHistoryRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ExamHistoryDAO>> DynamicFilter(IQueryable<ExamHistoryDAO> query, ExamHistoryFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Times, filter.Times);
            query = query.Where(q => q.CorrectAnswerQuantity, filter.CorrectAnswerQuantity);
            query = query.Where(q => q.TotalQuestionQuantity, filter.TotalQuestionQuantity);
            query = query.Where(q => q.Mark, filter.Mark);
            query = query.Where(q => q.ExamedAt, filter.ExamedAt);
            query = query.Where(q => q.AppUserId, filter.AppUserId);
            query = query.Where(q => q.ExamId, filter.ExamId);

            return query;
        }

        private async Task<IQueryable<ExamHistoryDAO>> OrFilter(IQueryable<ExamHistoryDAO> query, ExamHistoryFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ExamHistoryDAO> initQuery = query.Where(q => false);
            foreach (ExamHistoryFilter ExamHistoryFilter in filter.OrFilter)
            {
                IQueryable<ExamHistoryDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ExamHistoryFilter.Id);
                queryable = queryable.Where(q => q.Times, ExamHistoryFilter.Times);
                queryable = queryable.Where(q => q.CorrectAnswerQuantity, ExamHistoryFilter.CorrectAnswerQuantity);
                queryable = queryable.Where(q => q.TotalQuestionQuantity, ExamHistoryFilter.TotalQuestionQuantity);
                queryable = queryable.Where(q => q.Mark, ExamHistoryFilter.Mark);
                queryable = queryable.Where(q => q.ExamedAt, ExamHistoryFilter.ExamedAt);
                queryable = queryable.Where(q => q.AppUserId, ExamHistoryFilter.AppUserId);
                queryable = queryable.Where(q => q.ExamId, ExamHistoryFilter.ExamId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ExamHistoryDAO> DynamicOrder(IQueryable<ExamHistoryDAO> query, ExamHistoryFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ExamHistoryOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ExamHistoryOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case ExamHistoryOrder.Exam:
                            query = query.OrderBy(q => q.ExamId);
                            break;
                        case ExamHistoryOrder.Times:
                            query = query.OrderBy(q => q.Times);
                            break;
                        case ExamHistoryOrder.CorrectAnswerQuantity:
                            query = query.OrderBy(q => q.CorrectAnswerQuantity);
                            break;
                        case ExamHistoryOrder.TotalQuestionQuantity:
                            query = query.OrderBy(q => q.TotalQuestionQuantity);
                            break;
                        case ExamHistoryOrder.Mark:
                            query = query.OrderBy(q => q.Mark);
                            break;
                        case ExamHistoryOrder.ExamedAt:
                            query = query.OrderBy(q => q.ExamedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ExamHistoryOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ExamHistoryOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case ExamHistoryOrder.Exam:
                            query = query.OrderByDescending(q => q.ExamId);
                            break;
                        case ExamHistoryOrder.Times:
                            query = query.OrderByDescending(q => q.Times);
                            break;
                        case ExamHistoryOrder.CorrectAnswerQuantity:
                            query = query.OrderByDescending(q => q.CorrectAnswerQuantity);
                            break;
                        case ExamHistoryOrder.TotalQuestionQuantity:
                            query = query.OrderByDescending(q => q.TotalQuestionQuantity);
                            break;
                        case ExamHistoryOrder.Mark:
                            query = query.OrderByDescending(q => q.Mark);
                            break;
                        case ExamHistoryOrder.ExamedAt:
                            query = query.OrderByDescending(q => q.ExamedAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ExamHistory>> DynamicSelect(IQueryable<ExamHistoryDAO> query, ExamHistoryFilter filter)
        {
            List<ExamHistory> ExamHistories = await query.Select(q => new ExamHistory()
            {
                Id = filter.Selects.Contains(ExamHistorySelect.Id) ? q.Id : default(long),
                AppUserId = filter.Selects.Contains(ExamHistorySelect.AppUser) ? q.AppUserId : default(long),
                ExamId = filter.Selects.Contains(ExamHistorySelect.Exam) ? q.ExamId : default(long),
                Times = filter.Selects.Contains(ExamHistorySelect.Times) ? q.Times : default(long),
                CorrectAnswerQuantity = filter.Selects.Contains(ExamHistorySelect.CorrectAnswerQuantity) ? q.CorrectAnswerQuantity : default(long),
                TotalQuestionQuantity = filter.Selects.Contains(ExamHistorySelect.TotalQuestionQuantity) ? q.TotalQuestionQuantity : default(long),
                Mark = filter.Selects.Contains(ExamHistorySelect.Mark) ? q.Mark : default(decimal),
                ExamedAt = filter.Selects.Contains(ExamHistorySelect.ExamedAt) ? q.ExamedAt : default(DateTime),
                AppUser = filter.Selects.Contains(ExamHistorySelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                    Password = q.AppUser.Password,
                    RefreshToken = q.AppUser.RefreshToken,
                    ImageId = q.AppUser.ImageId,
                } : null,
                Exam = filter.Selects.Contains(ExamHistorySelect.Exam) && q.Exam != null ? new Exam
                {
                    Id = q.Exam.Id,
                    Code = q.Exam.Code,
                    Name = q.Exam.Name,
                    SubjectId = q.Exam.SubjectId,
                    ExamLevelId = q.Exam.ExamLevelId,
                    StatusId = q.Exam.StatusId,
                    CreatorId = q.Exam.CreatorId,
                    GradeId = q.Exam.GradeId,
                    ExamStatusId = q.Exam.ExamStatusId,
                    TotalMark = q.Exam.TotalMark,
                    TotalQuestion = q.Exam.TotalQuestion,
                    ImageId = q.Exam.ImageId,
                    Time = q.Exam.Time,
                } : null,
            }).ToListAsync();
            return ExamHistories;
        }

        public async Task<int> CountAll(ExamHistoryFilter filter)
        {
            IQueryable<ExamHistoryDAO> ExamHistoryDAOs = DataContext.ExamHistory.AsNoTracking();
            ExamHistoryDAOs = await DynamicFilter(ExamHistoryDAOs, filter);
            return await ExamHistoryDAOs.CountAsync();
        }

        public async Task<int> Count(ExamHistoryFilter filter)
        {
            IQueryable<ExamHistoryDAO> ExamHistoryDAOs = DataContext.ExamHistory.AsNoTracking();
            ExamHistoryDAOs = await DynamicFilter(ExamHistoryDAOs, filter);
            ExamHistoryDAOs = await OrFilter(ExamHistoryDAOs, filter);
            return await ExamHistoryDAOs.CountAsync();
        }

        public async Task<List<ExamHistory>> List(ExamHistoryFilter filter)
        {
            if (filter == null) return new List<ExamHistory>();
            IQueryable<ExamHistoryDAO> ExamHistoryDAOs = DataContext.ExamHistory.AsNoTracking();
            ExamHistoryDAOs = await DynamicFilter(ExamHistoryDAOs, filter);
            ExamHistoryDAOs = await OrFilter(ExamHistoryDAOs, filter);
            ExamHistoryDAOs = DynamicOrder(ExamHistoryDAOs, filter);
            List<ExamHistory> ExamHistories = await DynamicSelect(ExamHistoryDAOs, filter);
            return ExamHistories;
        }

        public async Task<List<ExamHistory>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ExamHistoryDAO> query = DataContext.ExamHistory.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<ExamHistory> ExamHistories = await query.AsNoTracking()
            .Select(x => new ExamHistory()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                ExamId = x.ExamId,
                Times = x.Times,
                CorrectAnswerQuantity = x.CorrectAnswerQuantity,
                TotalQuestionQuantity = x.TotalQuestionQuantity,
                Mark = x.Mark,
                ExamedAt = x.ExamedAt,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Password = x.AppUser.Password,
                    RefreshToken = x.AppUser.RefreshToken,
                    ImageId = x.AppUser.ImageId,
                },
                Exam = x.Exam == null ? null : new Exam
                {
                    Id = x.Exam.Id,
                    Code = x.Exam.Code,
                    Name = x.Exam.Name,
                    SubjectId = x.Exam.SubjectId,
                    ExamLevelId = x.Exam.ExamLevelId,
                    StatusId = x.Exam.StatusId,
                    CreatorId = x.Exam.CreatorId,
                    GradeId = x.Exam.GradeId,
                    ExamStatusId = x.Exam.ExamStatusId,
                    TotalMark = x.Exam.TotalMark,
                    TotalQuestion = x.Exam.TotalQuestion,
                    ImageId = x.Exam.ImageId,
                    Time = x.Exam.Time,
                },
            }).ToListAsync();
            

            return ExamHistories;
        }

        public async Task<ExamHistory> Get(long Id)
        {
            ExamHistory ExamHistory = await DataContext.ExamHistory.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new ExamHistory()
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                ExamId = x.ExamId,
                Times = x.Times,
                CorrectAnswerQuantity = x.CorrectAnswerQuantity,
                TotalQuestionQuantity = x.TotalQuestionQuantity,
                Mark = x.Mark,
                ExamedAt = x.ExamedAt,
                AppUser = x.AppUser == null ? null : new AppUser
                {
                    Id = x.AppUser.Id,
                    Username = x.AppUser.Username,
                    DisplayName = x.AppUser.DisplayName,
                    Password = x.AppUser.Password,
                    RefreshToken = x.AppUser.RefreshToken,
                    ImageId = x.AppUser.ImageId,
                },
                Exam = x.Exam == null ? null : new Exam
                {
                    Id = x.Exam.Id,
                    Code = x.Exam.Code,
                    Name = x.Exam.Name,
                    SubjectId = x.Exam.SubjectId,
                    ExamLevelId = x.Exam.ExamLevelId,
                    StatusId = x.Exam.StatusId,
                    CreatorId = x.Exam.CreatorId,
                    GradeId = x.Exam.GradeId,
                    ExamStatusId = x.Exam.ExamStatusId,
                    TotalMark = x.Exam.TotalMark,
                    TotalQuestion = x.Exam.TotalQuestion,
                    ImageId = x.Exam.ImageId,
                    Time = x.Exam.Time,
                },
            }).FirstOrDefaultAsync();

            if (ExamHistory == null)
                return null;

            return ExamHistory;
        }
        public async Task<bool> Create(ExamHistory ExamHistory)
        {
            ExamHistoryDAO ExamHistoryDAO = new ExamHistoryDAO();
            ExamHistoryDAO.Id = ExamHistory.Id;
            ExamHistoryDAO.AppUserId = ExamHistory.AppUserId;
            ExamHistoryDAO.ExamId = ExamHistory.ExamId;
            ExamHistoryDAO.Times = ExamHistory.Times;
            ExamHistoryDAO.CorrectAnswerQuantity = ExamHistory.CorrectAnswerQuantity;
            ExamHistoryDAO.TotalQuestionQuantity = ExamHistory.TotalQuestionQuantity;
            ExamHistoryDAO.Mark = ExamHistory.Mark;
            ExamHistoryDAO.ExamedAt = ExamHistory.ExamedAt;
            DataContext.ExamHistory.Add(ExamHistoryDAO);
            await DataContext.SaveChangesAsync();
            ExamHistory.Id = ExamHistoryDAO.Id;
            await SaveReference(ExamHistory);
            return true;
        }

        public async Task<bool> Update(ExamHistory ExamHistory)
        {
            ExamHistoryDAO ExamHistoryDAO = DataContext.ExamHistory
                .Where(x => x.Id == ExamHistory.Id)
                .FirstOrDefault();
            if (ExamHistoryDAO == null)
                return false;
            ExamHistoryDAO.Id = ExamHistory.Id;
            ExamHistoryDAO.AppUserId = ExamHistory.AppUserId;
            ExamHistoryDAO.ExamId = ExamHistory.ExamId;
            ExamHistoryDAO.Times = ExamHistory.Times;
            ExamHistoryDAO.CorrectAnswerQuantity = ExamHistory.CorrectAnswerQuantity;
            ExamHistoryDAO.TotalQuestionQuantity = ExamHistory.TotalQuestionQuantity;
            ExamHistoryDAO.Mark = ExamHistory.Mark;
            ExamHistoryDAO.ExamedAt = ExamHistory.ExamedAt;
            await DataContext.SaveChangesAsync();
            await SaveReference(ExamHistory);
            return true;
        }

        public async Task<bool> Delete(ExamHistory ExamHistory)
        {
            await DataContext.ExamHistory
                .Where(x => x.Id == ExamHistory.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        public async Task<List<long>> BulkMerge(List<ExamHistory> ExamHistories)
        {
            IdFilter IdFilter = new IdFilter { In = ExamHistories.Select(x => x.Id).ToList() };
            List<ExamHistoryDAO> Inserts = new List<ExamHistoryDAO>();
            List<ExamHistoryDAO> Updates = new List<ExamHistoryDAO>();
            List<ExamHistoryDAO> DbExamHistoryDAOs = await DataContext.ExamHistory
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (ExamHistory ExamHistory in ExamHistories)
            {
                ExamHistoryDAO ExamHistoryDAO = DbExamHistoryDAOs
                        .Where(x => x.Id == ExamHistory.Id)
                        .FirstOrDefault();
                if (ExamHistoryDAO == null)
                {
                    ExamHistoryDAO = new ExamHistoryDAO();
                    Inserts.Add(ExamHistoryDAO);
                }
                else
                {
                    Updates.Add(ExamHistoryDAO);
                }
                ExamHistoryDAO.AppUserId = ExamHistory.AppUserId;
                ExamHistoryDAO.ExamId = ExamHistory.ExamId;
                ExamHistoryDAO.Times = ExamHistory.Times;
                ExamHistoryDAO.CorrectAnswerQuantity = ExamHistory.CorrectAnswerQuantity;
                ExamHistoryDAO.TotalQuestionQuantity = ExamHistory.TotalQuestionQuantity;
                ExamHistoryDAO.Mark = ExamHistory.Mark;
                ExamHistoryDAO.ExamedAt = ExamHistory.ExamedAt;
            }
            await DataContext.ExamHistory.BulkInsertAsync(Inserts);
            await DataContext.ExamHistory.BulkMergeAsync(Updates);
            var Ids = Inserts.Select(x => x.Id).ToList();
            Ids.AddRange(Updates.Select(x => x.Id));
            Ids = Ids.Distinct().ToList();
            return Ids;
        }
        
        public async Task<bool> BulkDelete(List<ExamHistory> ExamHistories)
        {
            List<long> Ids = ExamHistories.Select(x => x.Id).ToList();
            await DataContext.ExamHistory
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ExamHistory ExamHistory)
        {
        }

    }
}
