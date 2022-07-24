using TrueSight.Common;
using Tracnghiem.Handlers.Configuration;
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

namespace Tracnghiem.Services.MQuestionContent
{
    public interface IQuestionContentService :  IServiceScoped
    {
        Task<int> Count(QuestionContentFilter QuestionContentFilter);
        Task<List<QuestionContent>> List(QuestionContentFilter QuestionContentFilter);
        Task<QuestionContent> Get(long Id);
        Task<QuestionContent> Create(QuestionContent QuestionContent);
        Task<QuestionContent> Update(QuestionContent QuestionContent);
        Task<QuestionContent> Delete(QuestionContent QuestionContent);
        Task<List<QuestionContent>> BulkDelete(List<QuestionContent> QuestionContents);
        Task<List<QuestionContent>> BulkMerge(List<QuestionContent> QuestionContents);
        Task<QuestionContentFilter> ToFilter(QuestionContentFilter QuestionContentFilter);
    }

    public class QuestionContentService : BaseService, IQuestionContentService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IQuestionContentValidator QuestionContentValidator;

        public QuestionContentService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IQuestionContentValidator QuestionContentValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.QuestionContentValidator = QuestionContentValidator;
        }

        public async Task<int> Count(QuestionContentFilter QuestionContentFilter)
        {
            try
            {
                int result = await UOW.QuestionContentRepository.Count(QuestionContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return 0;
        }

        public async Task<List<QuestionContent>> List(QuestionContentFilter QuestionContentFilter)
        {
            try
            {
                List<QuestionContent> QuestionContents = await UOW.QuestionContentRepository.List(QuestionContentFilter);
                return QuestionContents;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return null;
        }

        public async Task<QuestionContent> Get(long Id)
        {
            QuestionContent QuestionContent = await UOW.QuestionContentRepository.Get(Id);
            if (QuestionContent == null)
                return null;
            await QuestionContentValidator.Get(QuestionContent);
            return QuestionContent;
        }
        
        public async Task<QuestionContent> Create(QuestionContent QuestionContent)
        {
            if (!await QuestionContentValidator.Create(QuestionContent))
                return QuestionContent;

            try
            {
                await UOW.QuestionContentRepository.Create(QuestionContent);
                QuestionContent = await UOW.QuestionContentRepository.Get(QuestionContent.Id);
                Sync(new List<QuestionContent> { QuestionContent });
                return QuestionContent;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return null;
        }

        public async Task<QuestionContent> Update(QuestionContent QuestionContent)
        {
            if (!await QuestionContentValidator.Update(QuestionContent))
                return QuestionContent;
            try
            {
                var oldData = await UOW.QuestionContentRepository.Get(QuestionContent.Id);

                await UOW.QuestionContentRepository.Update(QuestionContent);

                QuestionContent = await UOW.QuestionContentRepository.Get(QuestionContent.Id);
                Sync(new List<QuestionContent> { QuestionContent });
                return QuestionContent;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return null;
        }

        public async Task<QuestionContent> Delete(QuestionContent QuestionContent)
        {
            if (!await QuestionContentValidator.Delete(QuestionContent))
                return QuestionContent;

            try
            {
                await UOW.QuestionContentRepository.Delete(QuestionContent);
                return QuestionContent;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return null;
        }

        public async Task<List<QuestionContent>> BulkDelete(List<QuestionContent> QuestionContents)
        {
            if (!await QuestionContentValidator.BulkDelete(QuestionContents))
                return QuestionContents;

            try
            {
                await UOW.QuestionContentRepository.BulkDelete(QuestionContents);
                return QuestionContents;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return null;
        }

        public async Task<List<QuestionContent>> BulkMerge(List<QuestionContent> QuestionContents)
        {
            if (!await QuestionContentValidator.Import(QuestionContents))
                return QuestionContents;
            try
            {
                var Ids = await UOW.QuestionContentRepository.BulkMerge(QuestionContents);
                QuestionContents = await UOW.QuestionContentRepository.List(Ids);
                Sync(QuestionContents);
                return QuestionContents;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionContentService));
            }
            return null;
        }     
        
        public async Task<QuestionContentFilter> ToFilter(QuestionContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<QuestionContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                QuestionContentFilter subFilter = new QuestionContentFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "QuestionContentId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.QuestionId))
                        subFilter.QuestionId = FilterBuilder.Merge(subFilter.QuestionId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AnswerContent))
                        subFilter.AnswerContent = FilterBuilder.Merge(subFilter.AnswerContent, FilterPermissionDefinition.StringFilter);
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

        private void Sync(List<QuestionContent> QuestionContents)
        {
            RabbitManager.PublishList(QuestionContents, RoutingKeyEnum.QuestionContentSync.Code);


            
        }
    }
}
