using TrueSight.Common;
using Tracnghiem.Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Tracnghiem.Models;
using Tracnghiem.Repositories;
using System;

namespace Tracnghiem.Repositories
{
    public interface IUOW : IServiceScoped, IDisposable
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAppUserRepository AppUserRepository { get; }
        IExamRepository ExamRepository { get; }
        IExamHistoryRepository ExamHistoryRepository { get; }
        IExamLevelRepository ExamLevelRepository { get; }
        IExamStatusRepository ExamStatusRepository { get; }
        IGradeRepository GradeRepository { get; }
        IImageRepository ImageRepository { get; }
        IQuestionContentRepository QuestionContentRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        IQuestionGroupRepository QuestionGroupRepository { get; }
        IQuestionTypeRepository QuestionTypeRepository { get; }
        IRoleRepository RoleRepository { get; }
        IStatusRepository StatusRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ISiteRepository SiteRepository { get; }
        IMenuRepository MenuRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IFieldRepository FieldRepository { get; }
        IPermissionOperatorRepository PermissionOperatorRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAppUserRepository AppUserRepository { get; private set; }
        public IExamRepository ExamRepository { get; private set; }
        public IExamHistoryRepository ExamHistoryRepository { get; private set; }
        public IExamLevelRepository ExamLevelRepository { get; private set; }
        public IExamStatusRepository ExamStatusRepository { get; private set; }
        public IGradeRepository GradeRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }
        public IQuestionContentRepository QuestionContentRepository { get; private set; }
        public IQuestionRepository QuestionRepository { get; private set; }
        public IQuestionGroupRepository QuestionGroupRepository { get; private set; }
        public IQuestionTypeRepository QuestionTypeRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }
        public ISubjectRepository SubjectRepository { get; private set; }
        public ISiteRepository SiteRepository { get; private set; }
        public IMenuRepository MenuRepository { get; private set; }
        public IPermissionRepository PermissionRepository { get; private set; }
        public IFieldRepository FieldRepository { get; private set; }
        public IPermissionOperatorRepository PermissionOperatorRepository { get; private set; }

        public UOW(DataContext DataContext, IRedisStore RedisStore, IConfiguration Configuration)
        {
            this.DataContext = DataContext;

            AppUserRepository = new AppUserRepository(DataContext);
            ExamRepository = new ExamRepository(DataContext);
            ExamHistoryRepository = new ExamHistoryRepository(DataContext);
            ExamLevelRepository = new ExamLevelRepository(DataContext);
            ExamStatusRepository = new ExamStatusRepository(DataContext);
            GradeRepository = new GradeRepository(DataContext);
            ImageRepository = new ImageRepository(DataContext);
            QuestionContentRepository = new QuestionContentRepository(DataContext);
            QuestionRepository = new QuestionRepository(DataContext);
            QuestionGroupRepository = new QuestionGroupRepository(DataContext);
            QuestionTypeRepository = new QuestionTypeRepository(DataContext);
            StatusRepository = new StatusRepository(DataContext);
            SubjectRepository = new SubjectRepository(DataContext);
            RoleRepository = new RoleRepository(DataContext, RedisStore, Configuration);
            SiteRepository = new SiteRepository(DataContext, RedisStore, Configuration);
            MenuRepository = new MenuRepository(DataContext, RedisStore, Configuration);
            PermissionRepository = new PermissionRepository(DataContext, RedisStore, Configuration);
            FieldRepository = new FieldRepository(DataContext);
            PermissionOperatorRepository = new PermissionOperatorRepository(DataContext);
        }
        public async Task Begin()
        {
            return;
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            //DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            //DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.DataContext == null)
            {
                return;
            }

            this.DataContext.Dispose();
            this.DataContext = null;
        }
    }
}