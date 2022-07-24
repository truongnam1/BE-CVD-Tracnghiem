using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracnghiem;
using Tracnghiem.Common;
using Tracnghiem.Enums;
using Tracnghiem.Entities;
using Tracnghiem.Repositories;
using Tracnghiem.Helpers;

namespace Tracnghiem.Services.MQuestionContent
{
    public interface IQuestionContentValidator : IServiceScoped
    {
        Task Get(QuestionContent QuestionContent);
        Task<bool> Create(QuestionContent QuestionContent);
        Task<bool> Update(QuestionContent QuestionContent);
        Task<bool> Delete(QuestionContent QuestionContent);
        Task<bool> BulkDelete(List<QuestionContent> QuestionContents);
        Task<bool> Import(List<QuestionContent> QuestionContents);
    }

    public class QuestionContentValidator : IQuestionContentValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private QuestionContentMessage QuestionContentMessage;

        public QuestionContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.QuestionContentMessage = new QuestionContentMessage();
        }

        public async Task Get(QuestionContent QuestionContent)
        {
        }

        public async Task<bool> Create(QuestionContent QuestionContent)
        {
            await ValidateAnswerContent(QuestionContent);
            await ValidateIsRight(QuestionContent);
            await ValidateQuestion(QuestionContent);
            return QuestionContent.IsValidated;
        }

        public async Task<bool> Update(QuestionContent QuestionContent)
        {
            if (await ValidateId(QuestionContent))
            {
                await ValidateAnswerContent(QuestionContent);
                await ValidateIsRight(QuestionContent);
                await ValidateQuestion(QuestionContent);
            }
            return QuestionContent.IsValidated;
        }

        public async Task<bool> Delete(QuestionContent QuestionContent)
        {
            var oldData = await UOW.QuestionContentRepository.Get(QuestionContent.Id);
            if (oldData != null)
            {
            }
            else
            {
                QuestionContent.AddError(nameof(QuestionContentValidator), nameof(QuestionContent.Id), QuestionContentMessage.Error.IdNotExisted, QuestionContentMessage);
            }
            return QuestionContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<QuestionContent> QuestionContents)
        {
            return QuestionContents.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<QuestionContent> QuestionContents)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(QuestionContent QuestionContent)
        {
            QuestionContentFilter QuestionContentFilter = new QuestionContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = QuestionContent.Id },
                Selects = QuestionContentSelect.Id
            };

            int count = await UOW.QuestionContentRepository.CountAll(QuestionContentFilter);
            if (count == 0)
                QuestionContent.AddError(nameof(QuestionContentValidator), nameof(QuestionContent.Id), QuestionContentMessage.Error.IdNotExisted, QuestionContentMessage);
            return QuestionContent.IsValidated;
        }
        private async Task<bool> ValidateAnswerContent(QuestionContent QuestionContent)
        {
            if (string.IsNullOrEmpty(QuestionContent.AnswerContent))
            {
                QuestionContent.AddError(nameof(QuestionContentValidator), nameof(QuestionContent.AnswerContent), QuestionContentMessage.Error.AnswerContentEmpty, QuestionContentMessage);
            }
            else if (QuestionContent.AnswerContent.Count() > 500)
            {
                QuestionContent.AddError(nameof(QuestionContentValidator), nameof(QuestionContent.AnswerContent), QuestionContentMessage.Error.AnswerContentOverLength, QuestionContentMessage);
            }
            return QuestionContent.IsValidated;
        }
        private async Task<bool> ValidateIsRight(QuestionContent QuestionContent)
        {   
            return true;
        }
        private async Task<bool> ValidateQuestion(QuestionContent QuestionContent)
        {       
            if(QuestionContent.QuestionId == 0)
            {
                QuestionContent.AddError(nameof(QuestionContentValidator), nameof(QuestionContent.Question), QuestionContentMessage.Error.QuestionEmpty, QuestionContentMessage);
            }
            else
            {
                int count = await UOW.QuestionRepository.CountAll(new QuestionFilter
                {
                    Id = new IdFilter{ Equal =  QuestionContent.QuestionId },
                });
                if(count == 0)
                {
                    QuestionContent.AddError(nameof(QuestionContentValidator), nameof(QuestionContent.Question), QuestionContentMessage.Error.QuestionNotExisted, QuestionContentMessage);
                }
            }
            return true;
        }
    }
}
