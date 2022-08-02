using TrueSight.Common;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Tracnghiem.Repositories;
using Tracnghiem.Entities;
using Tracnghiem.Enums;

namespace Tracnghiem.Services.MExam
{
    public interface IExamService : IServiceScoped
    {
        Task<int> Count(ExamFilter ExamFilter);
        Task<List<Exam>> List(ExamFilter ExamFilter);
        Task<Exam> Get(long Id);
        Task<Exam> PublicGet(long Id);

        Task<Exam> Create(Exam Exam);
        Task<Exam> Update(Exam Exam);
        Task<Exam> Delete(Exam Exam);
        Task<List<Exam>> BulkDelete(List<Exam> Exams);
        Task<List<Exam>> BulkMerge(List<Exam> Exams);
        Task<List<Exam>> MonthMostTested();

        Task<ExamFilter> ToFilter(ExamFilter ExamFilter);
    }

    public class ExamService : BaseService, IExamService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        private IExamValidator ExamValidator;

        public ExamService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IExamValidator ExamValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;

            this.ExamValidator = ExamValidator;
        }

        public async Task<int> Count(ExamFilter ExamFilter)
        {
            try
            {
                int result = await UOW.ExamRepository.Count(ExamFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return 0;
        }

        public async Task<List<Exam>> List(ExamFilter ExamFilter)
        {
            try
            {
                List<Exam> Exams = await UOW.ExamRepository.List(ExamFilter);
                return Exams;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return null;
        }

        public async Task<Exam> Get(long Id)
        {
            Exam Exam = await UOW.ExamRepository.Get(Id);
            if (Exam == null)
                return null;
            await ExamValidator.Get(Exam);
            return Exam;
        }

        public async Task<Exam> PublicGet(long Id)
        {
            Exam Exam = await UOW.ExamRepository.Get(Id);
            if (Exam == null || !(Exam.ExamStatusId == ExamStatusEnum.Public.Id))
                return null;
            await ExamValidator.Get(Exam);
            return Exam;
        }


        public async Task<Exam> Create(Exam Exam)
        {
            if (!await ExamValidator.Create(Exam))
                return Exam;

            try
            {
                Exam.CreatorId = CurrentContext.UserId;
                //Exam.ExamStatusId = ExamStatusEnum.Draft.Id;
                Exam.Code = string.Empty;
                Exam.TotalQuestion = TotalQuestion(Exam);
                await UOW.ExamRepository.Create(Exam);
                Exam = await UOW.ExamRepository.Get(Exam.Id);
                Exam.Code = $"Exam_{Exam.Id}";
                await UOW.ExamRepository.Update(Exam);
                Exam = await UOW.ExamRepository.Get(Exam.Id);

                return Exam;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return null;
        }

        public async Task<Exam> Update(Exam Exam)
        {
            if (!await ExamValidator.Update(Exam))
                return Exam;
            try
            {
                var oldData = await UOW.ExamRepository.Get(Exam.Id);
                oldData.TotalQuestion = TotalQuestion(Exam);
                await UOW.ExamRepository.Update(Exam);

                Exam = await UOW.ExamRepository.Get(Exam.Id);
                return Exam;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return null;
        }

        public async Task<Exam> Delete(Exam Exam)
        {
            if (!await ExamValidator.Delete(Exam))
                return Exam;

            try
            {
                await UOW.ExamRepository.Delete(Exam);
                return Exam;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return null;
        }

        public async Task<List<Exam>> BulkDelete(List<Exam> Exams)
        {
            if (!await ExamValidator.BulkDelete(Exams))
                return Exams;

            try
            {
                await UOW.ExamRepository.BulkDelete(Exams);
                return Exams;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return null;
        }

        public async Task<List<Exam>> BulkMerge(List<Exam> Exams)
        {
            if (!await ExamValidator.Import(Exams))
                return Exams;
            try
            {
                var Ids = await UOW.ExamRepository.BulkMerge(Exams);
                Exams = await UOW.ExamRepository.List(Ids);
                return Exams;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));
            }
            return null;
        }
        public async Task<List<Exam>> MonthMostTested()
        {
            try
            {
                ExamFilter ExamFilter = new ExamFilter();
                ExamFilter.Skip = 0;
                ExamFilter.Take = 10;
                ExamFilter.OrderBy = ExamOrder.CurrentMonthNumberTest;
                ExamFilter.OrderType = OrderType.DESC;
                ExamFilter.ExamStatusId = new IdFilter { Equal = ExamStatusEnum.Public.Id };
                ExamFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
                List<Exam> Exams = await UOW.ExamRepository.List(ExamFilter);
                return Exams;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamService));

            }

            return null;
        }

        public async Task<ExamFilter> ToFilter(ExamFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ExamFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ExamFilter subFilter = new ExamFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "ExamId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SubjectId))
                        subFilter.SubjectId = FilterBuilder.Merge(subFilter.SubjectId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ExamLevelId))
                        subFilter.ExamLevelId = FilterBuilder.Merge(subFilter.ExamLevelId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GradeId))
                        subFilter.GradeId = FilterBuilder.Merge(subFilter.GradeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ExamStatusId))
                        subFilter.ExamStatusId = FilterBuilder.Merge(subFilter.ExamStatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TotalMark))
                        subFilter.TotalMark = FilterBuilder.Merge(subFilter.TotalMark, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TotalQuestion))
                        subFilter.TotalQuestion = FilterBuilder.Merge(subFilter.TotalQuestion, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ImageId))
                        subFilter.ImageId = FilterBuilder.Merge(subFilter.ImageId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Time))
                        subFilter.Time = FilterBuilder.Merge(subFilter.Time, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
        private long TotalQuestion(Exam Exam)
        {
            if (Exam.ExamQuestionMappings == null || Exam.ExamQuestionMappings.Count() == 0)
                return 0;
            else
            {
                return Exam.ExamQuestionMappings.Count();
            }
        }
    }
}
