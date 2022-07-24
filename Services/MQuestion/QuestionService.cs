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

namespace Tracnghiem.Services.MQuestion
{
    public interface IQuestionService :  IServiceScoped
    {
        Task<int> Count(QuestionFilter QuestionFilter);
        Task<List<Question>> List(QuestionFilter QuestionFilter);
        Task<Question> Get(long Id);
        Task<Question> Create(Question Question);
        Task<Question> Update(Question Question);
        Task<Question> Delete(Question Question);
        Task<List<Question>> BulkDelete(List<Question> Questions);
        Task<List<Question>> BulkMerge(List<Question> Questions);
        Task<QuestionFilter> ToFilter(QuestionFilter QuestionFilter);
    }

    public class QuestionService : BaseService, IQuestionService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IQuestionValidator QuestionValidator;

        public QuestionService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IQuestionValidator QuestionValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.QuestionValidator = QuestionValidator;
        }

        public async Task<int> Count(QuestionFilter QuestionFilter)
        {
            try
            {
                int result = await UOW.QuestionRepository.Count(QuestionFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return 0;
        }

        public async Task<List<Question>> List(QuestionFilter QuestionFilter)
        {
            try
            {
                List<Question> Questions = await UOW.QuestionRepository.List(QuestionFilter);
                return Questions;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<Question> Get(long Id)
        {
            Question Question = await UOW.QuestionRepository.Get(Id);
            if (Question == null)
                return null;
            await QuestionValidator.Get(Question);
            return Question;
        }
        
        public async Task<Question> Create(Question Question)
        {
            if (!await QuestionValidator.Create(Question))
                return Question;

            try
            {
                await UOW.QuestionRepository.Create(Question);
                Question = await UOW.QuestionRepository.Get(Question.Id);
                return Question;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<Question> Update(Question Question)
        {
            if (!await QuestionValidator.Update(Question))
                return Question;
            try
            {
                var oldData = await UOW.QuestionRepository.Get(Question.Id);

                await UOW.QuestionRepository.Update(Question);

                Question = await UOW.QuestionRepository.Get(Question.Id);
                return Question;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<Question> Delete(Question Question)
        {
            if (!await QuestionValidator.Delete(Question))
                return Question;

            try
            {
                await UOW.QuestionRepository.Delete(Question);
                return Question;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<List<Question>> BulkDelete(List<Question> Questions)
        {
            if (!await QuestionValidator.BulkDelete(Questions))
                return Questions;

            try
            {
                await UOW.QuestionRepository.BulkDelete(Questions);
                return Questions;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }

        public async Task<List<Question>> BulkMerge(List<Question> Questions)
        {
            if (!await QuestionValidator.Import(Questions))
                return Questions;
            try
            {
                var Ids = await UOW.QuestionRepository.BulkMerge(Questions);
                Questions = await UOW.QuestionRepository.List(Ids);
                return Questions;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(QuestionService));
            }
            return null;
        }     
        
        public async Task<QuestionFilter> ToFilter(QuestionFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<QuestionFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                QuestionFilter subFilter = new QuestionFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "QuestionId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SubjectId))
                        subFilter.SubjectId = FilterBuilder.Merge(subFilter.SubjectId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.QuestionGroupId))
                        subFilter.QuestionGroupId = FilterBuilder.Merge(subFilter.QuestionGroupId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.QuestionTypeId))
                        subFilter.QuestionTypeId = FilterBuilder.Merge(subFilter.QuestionTypeId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Content))
                        subFilter.Content = FilterBuilder.Merge(subFilter.Content, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = FilterBuilder.Merge(subFilter.CreatorId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.GradeId))
                        subFilter.GradeId = FilterBuilder.Merge(subFilter.GradeId, FilterPermissionDefinition.IdFilter);
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
