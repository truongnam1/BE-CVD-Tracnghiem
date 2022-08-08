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

namespace Tracnghiem.Services.MQuestion
{
    public interface IQuestionValidator : IServiceScoped
    {
        Task Get(Question Question);
        Task<bool> Create(Question Question);
        Task<bool> Update(Question Question);
        Task<bool> Delete(Question Question);
        Task<bool> BulkDelete(List<Question> Questions);
        Task<bool> Import(List<Question> Questions);
    }

    public class QuestionValidator : IQuestionValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private QuestionMessage QuestionMessage;

        public QuestionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.QuestionMessage = new QuestionMessage();
        }

        public async Task Get(Question Question)
        {
        }

        public async Task<bool> Create(Question Question)
        {
            //await ValidateCode(Question);
            //await ValidateName(Question);
            await ValidateContent(Question);
            await ValidateGrade(Question);
            await ValidateQuestionGroup(Question);
            await ValidateQuestionType(Question);
            await ValidateStatus(Question);
            await ValidateSubject(Question);
            await ValidateQuestionContents(Question);
            return Question.IsValidated;
        }

        public async Task<bool> Update(Question Question)
        {
            if (await ValidateId(Question))
            {
                //await ValidateCode(Question);
                //await ValidateName(Question);
                await ValidateContent(Question);
                await ValidateGrade(Question);
                await ValidateQuestionGroup(Question);
                await ValidateQuestionType(Question);
                await ValidateStatus(Question);
                await ValidateSubject(Question);
                await ValidateQuestionContents(Question);
            }
            return Question.IsValidated;
        }

        public async Task<bool> Delete(Question Question)
        {
            var oldData = await UOW.QuestionRepository.Get(Question.Id);
            if (oldData != null)
            {
            }
            else
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Id), QuestionMessage.Error.IdNotExisted, QuestionMessage);
            }
            return Question.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Question> Questions)
        {
            return Questions.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<Question> Questions)
        {
            return true;
        }

        private async Task<bool> ValidateId(Question Question)
        {
            QuestionFilter QuestionFilter = new QuestionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Question.Id },
                Selects = QuestionSelect.Id
            };

            int count = await UOW.QuestionRepository.CountAll(QuestionFilter);
            if (count == 0)
                Question.AddError(nameof(QuestionValidator), nameof(Question.Id), QuestionMessage.Error.IdNotExisted, QuestionMessage);
            return Question.IsValidated;
        }
        private async Task<bool> ValidateCode(Question Question)
        {
            if (string.IsNullOrEmpty(Question.Code))
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Code), QuestionMessage.Error.CodeEmpty, QuestionMessage);
            }
            else
            {
                if (Question.Code.Contains(" "))
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.Code), QuestionMessage.Error.CodeHasSpecialCharacter, QuestionMessage);
                }
                if (Question.Code.Count() > 500)
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.Code), QuestionMessage.Error.CodeOverLength, QuestionMessage);
                }
                QuestionFilter QuestionFilter = new QuestionFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Question.Id },
                    Code = new StringFilter { Equal = Question.Code },
                    Selects = QuestionSelect.Code
                };

                int count = await UOW.QuestionRepository.Count(QuestionFilter);
                if (count != 0)
                    Question.AddError(nameof(QuestionValidator), nameof(Question.Code), QuestionMessage.Error.CodeExisted);
            }

            return Question.IsValidated;
        }
        private async Task<bool> ValidateName(Question Question)
        {
            if (string.IsNullOrEmpty(Question.Name))
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Name), QuestionMessage.Error.NameEmpty, QuestionMessage);
            }
            else if (Question.Name.Count() > 500)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Name), QuestionMessage.Error.NameOverLength, QuestionMessage);
            }
            return Question.IsValidated;
        }
        private async Task<bool> ValidateContent(Question Question)
        {
            if (string.IsNullOrEmpty(Question.Content))
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Content), QuestionMessage.Error.ContentEmpty, QuestionMessage);
            }
            else if (Question.Content.Count() > 500)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Content), QuestionMessage.Error.ContentOverLength, QuestionMessage);
            }
            return Question.IsValidated;
        }
        private async Task<bool> ValidateGrade(Question Question)
        {
            if (Question.GradeId == 0)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Grade), QuestionMessage.Error.GradeEmpty, QuestionMessage);
            }
            else
            {
                if (!GradeEnum.GradeEnumList.Any(x => Question.GradeId == x.Id))
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.Grade), QuestionMessage.Error.GradeNotExisted, QuestionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateQuestionGroup(Question Question)
        {
            if (Question.QuestionGroupId == 0)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionGroup), QuestionMessage.Error.QuestionGroupEmpty, QuestionMessage);
            }
            else
            {
                if (!QuestionGroupEnum.QuestionGroupEnumList.Any(x => Question.QuestionGroupId == x.Id))
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionGroup), QuestionMessage.Error.QuestionGroupNotExisted, QuestionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateQuestionType(Question Question)
        {
            if (Question.QuestionTypeId == 0)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionType), QuestionMessage.Error.QuestionTypeEmpty, QuestionMessage);
            }
            else
            {
                if (!QuestionTypeEnum.QuestionTypeEnumList.Any(x => Question.QuestionTypeId == x.Id))
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionType), QuestionMessage.Error.QuestionTypeNotExisted, QuestionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateStatus(Question Question)
        {
            if (Question.StatusId == 0)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Status), QuestionMessage.Error.StatusEmpty, QuestionMessage);
            }
            else
            {
                if (!StatusEnum.StatusEnumList.Any(x => Question.StatusId == x.Id))
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.Status), QuestionMessage.Error.StatusNotExisted, QuestionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateSubject(Question Question)
        {
            if (Question.SubjectId == 0)
            {
                Question.AddError(nameof(QuestionValidator), nameof(Question.Subject), QuestionMessage.Error.SubjectEmpty, QuestionMessage);
            }
            else
            {
                if (!SubjectEnum.SubjectEnumList.Any(x => Question.SubjectId == x.Id))
                {
                    Question.AddError(nameof(QuestionValidator), nameof(Question.Subject), QuestionMessage.Error.SubjectNotExisted, QuestionMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateQuestionContents(Question Question)
        {
            if (Question.QuestionContents != null && Question.QuestionContents.Count() > 0)
            {
                int count = Question.QuestionContents.Where(x => x.IsRight == true).Count();

                foreach (QuestionContent QuestionContent in Question.QuestionContents)
                {
                    if (string.IsNullOrEmpty(QuestionContent.AnswerContent))
                    {
                        QuestionContent.AddError(nameof(QuestionValidator), nameof(QuestionContent.AnswerContent), QuestionMessage.Error.QuestionContent_AnswerContentEmpty, QuestionMessage);
                    }
                    else if (QuestionContent.AnswerContent.Count() > 500)
                    {
                        QuestionContent.AddError(nameof(QuestionValidator), nameof(QuestionContent.AnswerContent), QuestionMessage.Error.QuestionContent_AnswerContentOverLength, QuestionMessage);
                    }

                }

                if (QuestionTypeEnum.ChooseOneAnswer.Id == Question.QuestionTypeId)
                {
                    if (count > 1)
                    {
                        Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionContents), QuestionMessage.Error.QuestionContent_OverCorectAnswers, QuestionMessage);
                    }
                    else if (count == 0)
                    {
                        Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionContents), QuestionMessage.Error.QuestionContent_NotCorectAnswers, QuestionMessage);
                    }
                }
                else if (QuestionTypeEnum.ChooseMultipleAnswer.Id == Question.QuestionTypeId)
                {
                    if (count == 0)
                    {
                        Question.AddError(nameof(QuestionValidator), nameof(Question.QuestionContents), QuestionMessage.Error.QuestionContent_NotCorectAnswers, QuestionMessage);
                    }
                }
              
            }
            return true;
        }
    }
}
