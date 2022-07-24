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

namespace Tracnghiem.Services.MExamHistory
{
    public interface IExamHistoryValidator : IServiceScoped
    {
        Task Get(ExamHistory ExamHistory);
        Task<bool> Create(ExamHistory ExamHistory);
        Task<bool> Update(ExamHistory ExamHistory);
        Task<bool> Delete(ExamHistory ExamHistory);
        Task<bool> BulkDelete(List<ExamHistory> ExamHistories);
        Task<bool> Import(List<ExamHistory> ExamHistories);
    }

    public class ExamHistoryValidator : IExamHistoryValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ExamHistoryMessage ExamHistoryMessage;

        public ExamHistoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ExamHistoryMessage = new ExamHistoryMessage();
        }

        public async Task Get(ExamHistory ExamHistory)
        {
        }

        public async Task<bool> Create(ExamHistory ExamHistory)
        {
            await ValidateTimes(ExamHistory);
            await ValidateCorrectAnswerQuantity(ExamHistory);
            await ValidateTotalQuestionQuantity(ExamHistory);
            await ValidateMark(ExamHistory);
            await ValidateExamedAt(ExamHistory);
            await ValidateAppUser(ExamHistory);
            await ValidateExam(ExamHistory);
            return ExamHistory.IsValidated;
        }

        public async Task<bool> Update(ExamHistory ExamHistory)
        {
            if (await ValidateId(ExamHistory))
            {
                await ValidateTimes(ExamHistory);
                await ValidateCorrectAnswerQuantity(ExamHistory);
                await ValidateTotalQuestionQuantity(ExamHistory);
                await ValidateMark(ExamHistory);
                await ValidateExamedAt(ExamHistory);
                await ValidateAppUser(ExamHistory);
                await ValidateExam(ExamHistory);
            }
            return ExamHistory.IsValidated;
        }

        public async Task<bool> Delete(ExamHistory ExamHistory)
        {
            var oldData = await UOW.ExamHistoryRepository.Get(ExamHistory.Id);
            if (oldData != null)
            {
            }
            else
            {
                ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.Id), ExamHistoryMessage.Error.IdNotExisted, ExamHistoryMessage);
            }
            return ExamHistory.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ExamHistory> ExamHistories)
        {
            return ExamHistories.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<ExamHistory> ExamHistories)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(ExamHistory ExamHistory)
        {
            ExamHistoryFilter ExamHistoryFilter = new ExamHistoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ExamHistory.Id },
                Selects = ExamHistorySelect.Id
            };

            int count = await UOW.ExamHistoryRepository.CountAll(ExamHistoryFilter);
            if (count == 0)
                ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.Id), ExamHistoryMessage.Error.IdNotExisted, ExamHistoryMessage);
            return ExamHistory.IsValidated;
        }
        private async Task<bool> ValidateTimes(ExamHistory ExamHistory)
        {   
            return true;
        }
        private async Task<bool> ValidateCorrectAnswerQuantity(ExamHistory ExamHistory)
        {   
            return true;
        }
        private async Task<bool> ValidateTotalQuestionQuantity(ExamHistory ExamHistory)
        {   
            return true;
        }
        private async Task<bool> ValidateMark(ExamHistory ExamHistory)
        {   
            return true;
        }
        private async Task<bool> ValidateExamedAt(ExamHistory ExamHistory)
        {       
            if(ExamHistory.ExamedAt <= new DateTime(2000, 1, 1))
            {
                ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.ExamedAt), ExamHistoryMessage.Error.ExamedAtEmpty, ExamHistoryMessage);
            }
            return true;
        }
        private async Task<bool> ValidateAppUser(ExamHistory ExamHistory)
        {       
            if(ExamHistory.AppUserId == 0)
            {
                ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.AppUser), ExamHistoryMessage.Error.AppUserEmpty, ExamHistoryMessage);
            }
            else
            {
                int count = await UOW.AppUserRepository.CountAll(new AppUserFilter
                {
                    Id = new IdFilter{ Equal =  ExamHistory.AppUserId },
                });
                if(count == 0)
                {
                    ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.AppUser), ExamHistoryMessage.Error.AppUserNotExisted, ExamHistoryMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateExam(ExamHistory ExamHistory)
        {       
            if(ExamHistory.ExamId == 0)
            {
                ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.Exam), ExamHistoryMessage.Error.ExamEmpty, ExamHistoryMessage);
            }
            else
            {
                int count = await UOW.ExamRepository.CountAll(new ExamFilter
                {
                    Id = new IdFilter{ Equal =  ExamHistory.ExamId },
                });
                if(count == 0)
                {
                    ExamHistory.AddError(nameof(ExamHistoryValidator), nameof(ExamHistory.Exam), ExamHistoryMessage.Error.ExamNotExisted, ExamHistoryMessage);
                }
            }
            return true;
        }
    }
}
