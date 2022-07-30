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

namespace Tracnghiem.Services.MExam
{
    public interface IExamValidator : IServiceScoped
    {
        Task Get(Exam Exam);
        Task<bool> Create(Exam Exam);
        Task<bool> Update(Exam Exam);
        Task<bool> Delete(Exam Exam);
        Task<bool> BulkDelete(List<Exam> Exams);
        Task<bool> Import(List<Exam> Exams);
        Task<bool> Send(Exam Exam);

    }

    public class ExamValidator : IExamValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ExamMessage ExamMessage;

        public ExamValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ExamMessage = new ExamMessage();
        }

        public async Task Get(Exam Exam)
        {
        }

        public async Task<bool> Create(Exam Exam)
        {
            await ValidateName(Exam);
            await ValidateTotalMark(Exam);
            await ValidateTime(Exam);
            await ValidateExamLevel(Exam);
            await ValidateExamStatus(Exam);
            await ValidateGrade(Exam);
            await ValidateImage(Exam);
            await ValidateStatus(Exam);
            await ValidateSubject(Exam);
            await ValidateExamQuestionMappings(Exam);
            return Exam.IsValidated;
        }

        public async Task<bool> Update(Exam Exam)
        {
            if (await ValidateId(Exam))
            {
                await ValidateName(Exam);
                await ValidateTotalMark(Exam);
                await ValidateTime(Exam);
                await ValidateExamLevel(Exam);
                await ValidateExamStatus(Exam);
                await ValidateGrade(Exam);
                await ValidateImage(Exam);
                await ValidateStatus(Exam);
                await ValidateSubject(Exam);
                await ValidateExamQuestionMappings(Exam);
            }
            return Exam.IsValidated;
        }

        public async Task<bool> Delete(Exam Exam)
        {
            var oldData = await UOW.ExamRepository.Get(Exam.Id);
            if (oldData != null)
            {
            }
            else
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Id), ExamMessage.Error.IdNotExisted, ExamMessage);
            }
            return Exam.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Exam> Exams)
        {
            return Exams.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<Exam> Exams)
        {
            return true;
        }
        public async Task<bool> Send(Exam Exam)
        {
            await ValidateId(Exam);
            return Exam.IsValidated;
        }
        private async Task<bool> ValidateId(Exam Exam)
        {
            ExamFilter ExamFilter = new ExamFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Exam.Id },
                Selects = ExamSelect.Id
            };

            int count = await UOW.ExamRepository.CountAll(ExamFilter);
            if (count == 0)
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Id), ExamMessage.Error.IdNotExisted, ExamMessage);
            return Exam.IsValidated;
        }
         private async Task<bool> ValidateCode(Exam Exam)
         {
            if (string.IsNullOrEmpty(Exam.Code))
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Code), ExamMessage.Error.CodeEmpty, ExamMessage);
            }
            else
            {
                if (Exam.Code.Contains(" "))
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Code), ExamMessage.Error.CodeHasSpecialCharacter, ExamMessage);
                }
                if (Exam.Code.Count() > 500)
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Code), ExamMessage.Error.CodeOverLength, ExamMessage);
                }
                ExamFilter ExamFilter = new ExamFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Exam.Id },
                    Code = new StringFilter { Equal = Exam.Code },
                    Selects = ExamSelect.Code
                };

                int count = await UOW.ExamRepository.Count(ExamFilter);
                if (count != 0)
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Code), ExamMessage.Error.CodeExisted);
            }

            return Exam.IsValidated;
        }
        private async Task<bool> ValidateName(Exam Exam)
        {
            if (string.IsNullOrEmpty(Exam.Name))
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Name), ExamMessage.Error.NameEmpty, ExamMessage);
            }
            else if (Exam.Name.Count() > 500)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Name), ExamMessage.Error.NameOverLength, ExamMessage);
            }
            return Exam.IsValidated;
        }
        private async Task<bool> ValidateTotalMark(Exam Exam)
        {
            if (Exam.TotalMark.HasValue)
            {
                if (Exam.TotalMark < 0)
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.TotalMark), ExamMessage.Error.TotalMarkInvalid, ExamMessage);

                }
                else
                {
                    if (Exam.ExamQuestionMappings != null && Exam.ExamQuestionMappings.Count() > 0)
                    {
                        decimal? totalMark = 0;
                        Exam.ExamQuestionMappings.ForEach(x => totalMark += x.Mark == null ? 0 : x.Mark);
                        if (totalMark != Exam.TotalMark)
                        {
                            Exam.AddError(nameof(ExamValidator), nameof(Exam.TotalMark), ExamMessage.Error.TotalMarkInvalid, ExamMessage);
                        }
                    }
                }
            }
            return true;
        }
        private async Task<bool> ValidateTime(Exam Exam)
        {
            if (Exam.Time.HasValue && Exam.Time <= 0)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Time), ExamMessage.Error.TimeInvalid, ExamMessage);
            }
            return true;
        }
        private async Task<bool> ValidateExamLevel(Exam Exam)
        {       
            if(Exam.ExamLevelId == 0)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.ExamLevel), ExamMessage.Error.ExamLevelEmpty, ExamMessage);
            }
            else
            {
                if(!ExamLevelEnum.ExamLevelEnumList.Any(x => Exam.ExamLevelId == x.Id))
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.ExamLevel), ExamMessage.Error.ExamLevelNotExisted, ExamMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateExamStatus(Exam Exam)
        {       
            if(Exam.ExamStatusId == 0)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.ExamStatus), ExamMessage.Error.ExamStatusEmpty, ExamMessage);
            }
            else
            {
                if(!ExamStatusEnum.ExamStatusEnumList.Any(x => Exam.ExamStatusId == x.Id))
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.ExamStatus), ExamMessage.Error.ExamStatusNotExisted, ExamMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateGrade(Exam Exam)
        {       
            if(Exam.GradeId == 0)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Grade), ExamMessage.Error.GradeEmpty, ExamMessage);
            }
            else
            {
                if(!GradeEnum.GradeEnumList.Any(x => Exam.GradeId == x.Id))
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Grade), ExamMessage.Error.GradeNotExisted, ExamMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateImage(Exam Exam)
        {       
            if(Exam.ImageId.HasValue)
            {
                int count = await UOW.ImageRepository.CountAll(new ImageFilter
                {
                    Id = new IdFilter{ Equal =  Exam.ImageId },
                });
                if(count == 0)
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Image), ExamMessage.Error.ImageNotExisted, ExamMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateStatus(Exam Exam)
        {       
            if(Exam.StatusId == 0)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Status), ExamMessage.Error.StatusEmpty, ExamMessage);
            }
            else
            {
                if(!StatusEnum.StatusEnumList.Any(x => Exam.StatusId == x.Id))
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Status), ExamMessage.Error.StatusNotExisted, ExamMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateSubject(Exam Exam)
        {       
            if(Exam.SubjectId == 0)
            {
                Exam.AddError(nameof(ExamValidator), nameof(Exam.Subject), ExamMessage.Error.SubjectEmpty, ExamMessage);
            }
            else
            {
                if(!SubjectEnum.SubjectEnumList.Any(x => Exam.SubjectId == x.Id))
                {
                    Exam.AddError(nameof(ExamValidator), nameof(Exam.Subject), ExamMessage.Error.SubjectNotExisted, ExamMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateExamQuestionMappings(Exam Exam)
        {   
            if(Exam.ExamQuestionMappings?.Any() ?? false)
            {
                #region fetch data
                List<long> QuestionIds = new List<long>();
                QuestionIds.AddRange(Exam.ExamQuestionMappings.Select(x => x.QuestionId));
                List<Question> Questions = await UOW.QuestionRepository.List(new QuestionFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = QuestionSelect.Id,

                    StatusId = new IdFilter{ Equal = StatusEnum.ACTIVE.Id },
                    Id = new IdFilter { In = QuestionIds },
                });
                #endregion

                #region validate
                foreach(ExamQuestionMapping ExamQuestionMapping in Exam.ExamQuestionMappings)
                {
                    if(ExamQuestionMapping.QuestionId == 0)
                    {
                        ExamQuestionMapping.AddError(nameof(ExamValidator), nameof(ExamQuestionMapping.Question), ExamMessage.Error.ExamQuestionMapping_QuestionEmpty, ExamMessage);
                    }
                    else
                    {
                        Question Question = Questions.FirstOrDefault(x => x.Id == ExamQuestionMapping.QuestionId);
                        if(Question == null)
                        {
                            ExamQuestionMapping.AddError(nameof(ExamValidator), nameof(ExamQuestionMapping.Question), ExamMessage.Error.ExamQuestionMapping_QuestionNotExisted, ExamMessage);
                        }

                        if (ExamQuestionMapping.Mark < 0)
                        {
                            ExamQuestionMapping.AddError(nameof(ExamValidator), nameof(ExamQuestionMapping.Mark), ExamMessage.Error.ExamQuestionMapping_MarkInvalid, ExamMessage);
                        }
                        if (Exam.TotalMark.HasValue && ExamQuestionMapping.Mark == null)
                        {
                            ExamQuestionMapping.AddError(nameof(ExamValidator), nameof(ExamQuestionMapping.Mark), ExamMessage.Error.ExamQuestionMapping_MarkInvalid, ExamMessage);
                        }
                    }
                    
                }
                #endregion
            }
            else 
            {

            }
            return true;
        }
    }
}
