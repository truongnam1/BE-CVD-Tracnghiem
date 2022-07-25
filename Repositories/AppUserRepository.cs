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
    public interface IAppUserRepository
    {
        Task<int> CountAll(AppUserFilter AppUserFilter);
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(List<long> Ids);
        Task<AppUser> Get(long Id);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<List<long>> BulkMerge(List<AppUser> AppUsers);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<AppUserDAO>> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Username, filter.Username);
            query = query.Where(q => q.DisplayName, filter.DisplayName);
            query = query.Where(q => q.Email, filter.Email);
            query = query.Where(q => q.Password, filter.Password);
            query = query.Where(q => q.RefreshToken, filter.RefreshToken);
            query = query.Where(q => q.ImageId, filter.ImageId);

            return query;
        }

        private async Task<IQueryable<AppUserDAO>> OrFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AppUserDAO> initQuery = query.Where(q => false);
            foreach (AppUserFilter AppUserFilter in filter.OrFilter)
            {
                IQueryable<AppUserDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, AppUserFilter.Id);
                queryable = queryable.Where(q => q.Username, AppUserFilter.Username);
                queryable = queryable.Where(q => q.DisplayName, AppUserFilter.DisplayName);
                queryable = queryable.Where(q => q.Password, AppUserFilter.Password);
                queryable = queryable.Where(q => q.RefreshToken, AppUserFilter.RefreshToken);
                queryable = queryable.Where(q => q.ImageId, AppUserFilter.ImageId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<AppUserDAO> DynamicOrder(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AppUserOrder.RefreshToken:
                            query = query.OrderBy(q => q.RefreshToken);
                            break;
                        case AppUserOrder.Image:
                            query = query.OrderBy(q => q.ImageId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AppUserOrder.RefreshToken:
                            query = query.OrderByDescending(q => q.RefreshToken);
                            break;
                        case AppUserOrder.Image:
                            query = query.OrderByDescending(q => q.ImageId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AppUser>> DynamicSelect(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            List<AppUser> AppUsers = await query.Select(q => new AppUser()
            {
                Id = filter.Selects.Contains(AppUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AppUserSelect.Username) ? q.Username : default(string),
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Password = filter.Selects.Contains(AppUserSelect.Password) ? q.Password : default(string),
                RefreshToken = filter.Selects.Contains(AppUserSelect.RefreshToken) ? q.RefreshToken : default(string),
                ImageId = filter.Selects.Contains(AppUserSelect.Image) ? q.ImageId : default(long?),
                Image = filter.Selects.Contains(AppUserSelect.Image) && q.Image != null ? new Image
                {
                    Id = q.Image.Id,
                    Name = q.Image.Name,
                    Url = q.Image.Url,
                } : null,
            }).ToListAsync();
            return AppUsers;
        }

        public async Task<int> CountAll(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = await DynamicFilter(AppUserDAOs, filter);
            return await AppUserDAOs.CountAsync();
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = await DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = await OrFilter(AppUserDAOs, filter);
            return await AppUserDAOs.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = await DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = await OrFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<List<AppUser>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<AppUserDAO> query = DataContext.AppUser.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<AppUser> AppUsers = await query.AsNoTracking()
            .Select(x => new AppUser()
            {
                Id = x.Id,
                Username = x.Username,
                DisplayName = x.DisplayName,
                Password = x.Password,
                RefreshToken = x.RefreshToken,
                ImageId = x.ImageId,
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
                }
            }).ToListAsync();
            
            var ExamHistoryQuery = DataContext.ExamHistory.AsNoTracking()
                .Where(x => x.AppUserId, IdFilter);
            List<ExamHistory> ExamHistories = await ExamHistoryQuery
                .Select(x => new ExamHistory
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    ExamId = x.ExamId,
                    Times = x.Times,
                    CorrectAnswerQuantity = x.CorrectAnswerQuantity,
                    TotalQuestionQuantity = x.TotalQuestionQuantity,
                    Mark = x.Mark,
                    ExamedAt = x.ExamedAt,
                    Exam = new Exam
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

            foreach(AppUser AppUser in AppUsers)
            {
                AppUser.ExamHistories = ExamHistories
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }


            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUser.AsNoTracking()
            .Where(x => x.Id == Id)
            .Select(x => new AppUser()
            {
                Id = x.Id,
                Username = x.Username,
                DisplayName = x.DisplayName,
                Password = x.Password,
                RefreshToken = x.RefreshToken,
                ImageId = x.ImageId,
                Image = x.Image == null ? null : new Image
                {
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    Url = x.Image.Url,
                }
            }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;
            AppUser.ExamHistories = await DataContext.ExamHistory.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id)
                .Select(x => new ExamHistory
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    ExamId = x.ExamId,
                    Times = x.Times,
                    CorrectAnswerQuantity = x.CorrectAnswerQuantity,
                    TotalQuestionQuantity = x.TotalQuestionQuantity,
                    Mark = x.Mark,
                    ExamedAt = x.ExamedAt,
                    Exam = new Exam
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

            return AppUser;
        }
        public async Task<bool> Create(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = new AppUserDAO();
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.RefreshToken = AppUser.RefreshToken;
            AppUserDAO.ImageId = AppUser.ImageId;
            AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            AppUserDAO.StatusId = StatusEnum.ACTIVE.Id;
            DataContext.AppUser.Add(AppUserDAO);
            await DataContext.SaveChangesAsync();
            AppUser.Id = AppUserDAO.Id;
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser
                .Where(x => x.Id == AppUser.Id)
                .FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.RefreshToken = AppUser.RefreshToken;
            AppUserDAO.ImageId = AppUser.ImageId;
            await DataContext.SaveChangesAsync();
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await DataContext.AppUser
                .Where(x => x.Id == AppUser.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        public async Task<List<long>> BulkMerge(List<AppUser> AppUsers)
        {
            IdFilter IdFilter = new IdFilter { In = AppUsers.Select(x => x.Id).ToList() };
            List<AppUserDAO> Inserts = new List<AppUserDAO>();
            List<AppUserDAO> Updates = new List<AppUserDAO>();
            List<AppUserDAO> DbAppUserDAOs = await DataContext.AppUser
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUserDAO AppUserDAO = DbAppUserDAOs
                        .Where(x => x.Id == AppUser.Id)
                        .FirstOrDefault();
                if (AppUserDAO == null)
                {
                    AppUserDAO = new AppUserDAO();
                    Inserts.Add(AppUserDAO);
                }
                else
                {
                    Updates.Add(AppUserDAO);
                }
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.Password = AppUser.Password;
                AppUserDAO.RefreshToken = AppUser.RefreshToken;
                AppUserDAO.ImageId = AppUser.ImageId;
            }
            await DataContext.AppUser.BulkInsertAsync(Inserts);
            await DataContext.AppUser.BulkMergeAsync(Updates);
            var Ids = Inserts.Select(x => x.Id).ToList();
            Ids.AddRange(Updates.Select(x => x.Id));
            Ids = Ids.Distinct().ToList();
            return Ids;
        }
        
        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUser
                .WhereBulkContains(Ids, x => x.Id)
                .DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(AppUser AppUser)
        {
            //await DataContext.ExamHistory
            //    .Where(x => x.AppUserId == AppUser.Id)
            //    .DeleteFromQueryAsync();
            //if (AppUser.ExamHistories != null)
            //{
            //    foreach (ExamHistory ExamHistory in AppUser.ExamHistories)
            //    {
            //        ExamHistoryDAO ExamHistoryDAO = new ExamHistoryDAO();
            //        ExamHistoryDAO.Id = ExamHistory.Id;
            //        ExamHistoryDAO.AppUserId = AppUser.Id;
            //        ExamHistoryDAO.ExamId = ExamHistory.ExamId;
            //        ExamHistoryDAO.Times = ExamHistory.Times;
            //        ExamHistoryDAO.CorrectAnswerQuantity = ExamHistory.CorrectAnswerQuantity;
            //        ExamHistoryDAO.TotalQuestionQuantity = ExamHistory.TotalQuestionQuantity;
            //        ExamHistoryDAO.Mark = ExamHistory.Mark;
            //        ExamHistoryDAO.ExamedAt = ExamHistory.ExamedAt;
            //        DataContext.ExamHistory.Add(ExamHistoryDAO);
            //    }
            //    await DataContext.SaveChangesAsync();
            //}
        }

    }
}
