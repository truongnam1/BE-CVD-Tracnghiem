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

namespace Tracnghiem.Services.MExamHistory
{
    public interface IExamHistoryService :  IServiceScoped
    {
        Task<int> Count(ExamHistoryFilter ExamHistoryFilter);
        Task<List<ExamHistory>> List(ExamHistoryFilter ExamHistoryFilter);
        Task<ExamHistory> Get(long Id);
        Task<ExamHistory> Create(ExamHistory ExamHistory);
        Task<ExamHistory> Update(ExamHistory ExamHistory);
        Task<ExamHistory> Delete(ExamHistory ExamHistory);
        Task<List<ExamHistory>> BulkDelete(List<ExamHistory> ExamHistories);
        Task<List<ExamHistory>> BulkMerge(List<ExamHistory> ExamHistories);
        Task<ExamHistoryFilter> ToFilter(ExamHistoryFilter ExamHistoryFilter);
    }

    public class ExamHistoryService : BaseService, IExamHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IExamHistoryValidator ExamHistoryValidator;

        public ExamHistoryService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IExamHistoryValidator ExamHistoryValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ExamHistoryValidator = ExamHistoryValidator;
        }

        public async Task<int> Count(ExamHistoryFilter ExamHistoryFilter)
        {
            try
            {
                int result = await UOW.ExamHistoryRepository.Count(ExamHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return 0;
        }

        public async Task<List<ExamHistory>> List(ExamHistoryFilter ExamHistoryFilter)
        {
            try
            {
                List<ExamHistory> ExamHistories = await UOW.ExamHistoryRepository.List(ExamHistoryFilter);
                return ExamHistories;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return null;
        }

        public async Task<ExamHistory> Get(long Id)
        {
            ExamHistory ExamHistory = await UOW.ExamHistoryRepository.Get(Id);
            if (ExamHistory == null)
                return null;
            await ExamHistoryValidator.Get(ExamHistory);
            return ExamHistory;
        }
        
        public async Task<ExamHistory> Create(ExamHistory ExamHistory)
        {
            if (!await ExamHistoryValidator.Create(ExamHistory))
                return ExamHistory;

            try
            {
                await UOW.ExamHistoryRepository.Create(ExamHistory);
                ExamHistory = await UOW.ExamHistoryRepository.Get(ExamHistory.Id);
                return ExamHistory;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return null;
        }

        public async Task<ExamHistory> Update(ExamHistory ExamHistory)
        {
            if (!await ExamHistoryValidator.Update(ExamHistory))
                return ExamHistory;
            try
            {
                var oldData = await UOW.ExamHistoryRepository.Get(ExamHistory.Id);

                await UOW.ExamHistoryRepository.Update(ExamHistory);

                ExamHistory = await UOW.ExamHistoryRepository.Get(ExamHistory.Id);
                return ExamHistory;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return null;
        }

        public async Task<ExamHistory> Delete(ExamHistory ExamHistory)
        {
            if (!await ExamHistoryValidator.Delete(ExamHistory))
                return ExamHistory;

            try
            {
                await UOW.ExamHistoryRepository.Delete(ExamHistory);
                return ExamHistory;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return null;
        }

        public async Task<List<ExamHistory>> BulkDelete(List<ExamHistory> ExamHistories)
        {
            if (!await ExamHistoryValidator.BulkDelete(ExamHistories))
                return ExamHistories;

            try
            {
                await UOW.ExamHistoryRepository.BulkDelete(ExamHistories);
                return ExamHistories;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return null;
        }

        public async Task<List<ExamHistory>> BulkMerge(List<ExamHistory> ExamHistories)
        {
            if (!await ExamHistoryValidator.Import(ExamHistories))
                return ExamHistories;
            try
            {
                var Ids = await UOW.ExamHistoryRepository.BulkMerge(ExamHistories);
                ExamHistories = await UOW.ExamHistoryRepository.List(Ids);
                return ExamHistories;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ExamHistoryService));
            }
            return null;
        }     
        
        public async Task<ExamHistoryFilter> ToFilter(ExamHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ExamHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ExamHistoryFilter subFilter = new ExamHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "ExamHistoryId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ExamId))
                        subFilter.ExamId = FilterBuilder.Merge(subFilter.ExamId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Times))
                        subFilter.Times = FilterBuilder.Merge(subFilter.Times, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CorrectAnswerQuantity))
                        subFilter.CorrectAnswerQuantity = FilterBuilder.Merge(subFilter.CorrectAnswerQuantity, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.TotalQuestionQuantity))
                        subFilter.TotalQuestionQuantity = FilterBuilder.Merge(subFilter.TotalQuestionQuantity, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Mark))
                        subFilter.Mark = FilterBuilder.Merge(subFilter.Mark, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ExamedAt))
                        subFilter.ExamedAt = FilterBuilder.Merge(subFilter.ExamedAt, FilterPermissionDefinition.DateFilter);
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
    }
}
