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

namespace Tracnghiem.Services.MAppUser
{
    public interface IAppUserValidator : IServiceScoped
    {
        Task Get(AppUser AppUser);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Import(List<AppUser> AppUsers);
    }

    public class AppUserValidator : IAppUserValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private AppUserMessage AppUserMessage;

        public AppUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.AppUserMessage = new AppUserMessage();
        }

        public async Task Get(AppUser AppUser)
        {
        }

        public async Task<bool> Create(AppUser AppUser)
        {
            await ValidateUsername(AppUser);
            await ValidateDisplayName(AppUser);
            await ValidatePassword(AppUser);
            await ValidateRefreshToken(AppUser);
            await ValidateImage(AppUser);
            await ValidateRole(AppUser);
            await ValidateExamHistories(AppUser);
            return AppUser.IsValidated;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
                await ValidateUsername(AppUser);
                await ValidateDisplayName(AppUser);
                await ValidatePassword(AppUser);
                await ValidateRefreshToken(AppUser);
                await ValidateImage(AppUser);
                await ValidateRole(AppUser);
                await ValidateExamHistories(AppUser);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
            if (oldData != null)
            {
            }
            else
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), AppUserMessage.Error.IdNotExisted, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            return AppUsers.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<AppUser> AppUsers)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.CountAll(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), AppUserMessage.Error.IdNotExisted, AppUserMessage);
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateUsername(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.Username))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UsernameEmpty, AppUserMessage);
            }
            else if (AppUser.Username.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), AppUserMessage.Error.UsernameOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateDisplayName(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.DisplayName))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), AppUserMessage.Error.DisplayNameEmpty, AppUserMessage);
            }
            else if (AppUser.DisplayName.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), AppUserMessage.Error.DisplayNameOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidatePassword(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.Password))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), AppUserMessage.Error.PasswordEmpty, AppUserMessage);
            }
            else if (AppUser.Password.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), AppUserMessage.Error.PasswordOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateRefreshToken(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.RefreshToken))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.RefreshToken), AppUserMessage.Error.RefreshTokenEmpty, AppUserMessage);
            }
            else if (AppUser.RefreshToken.Count() > 500)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.RefreshToken), AppUserMessage.Error.RefreshTokenOverLength, AppUserMessage);
            }
            return AppUser.IsValidated;
        }
        private async Task<bool> ValidateImage(AppUser AppUser)
        {       
            if(AppUser.ImageId.HasValue)
            {
                int count = await UOW.ImageRepository.CountAll(new ImageFilter
                {
                    Id = new IdFilter{ Equal =  AppUser.ImageId },
                });
                if(count == 0)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Image), AppUserMessage.Error.ImageNotExisted, AppUserMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateRole(AppUser AppUser)
        {       
            if(AppUser.RoleId == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Role), AppUserMessage.Error.RoleEmpty, AppUserMessage);
            }
            else
            {
                if(!RoleEnum.RoleEnumList.Any(x => AppUser.RoleId == x.Id))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Role), AppUserMessage.Error.RoleNotExisted, AppUserMessage);
                }
            }
            return true;
        }
        private async Task<bool> ValidateExamHistories(AppUser AppUser)
        {   
            if(AppUser.ExamHistories?.Any() ?? false)
            {
                #region fetch data
                List<long> ExamIds = new List<long>();
                ExamIds.AddRange(AppUser.ExamHistories.Select(x => x.ExamId));
                List<Exam> Exams = await UOW.ExamRepository.List(new ExamFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = ExamSelect.Id,

                    StatusId = new IdFilter{ Equal = StatusEnum.ACTIVE.Id },
                    Id = new IdFilter { In = ExamIds },
                });
                #endregion

                #region validate
                foreach(ExamHistory ExamHistory in AppUser.ExamHistories)
                {
                    if(ExamHistory.ExamedAt <= new DateTime(2000, 1, 1))
                    {
                        ExamHistory.AddError(nameof(AppUserValidator), nameof(ExamHistory.ExamedAt), AppUserMessage.Error.ExamHistory_ExamedAtEmpty, AppUserMessage);
                    }
                    
                    if(ExamHistory.ExamId == 0)
                    {
                        ExamHistory.AddError(nameof(AppUserValidator), nameof(ExamHistory.Exam), AppUserMessage.Error.ExamHistory_ExamEmpty, AppUserMessage);
                    }
                    else
                    {
                        Exam Exam = Exams.FirstOrDefault(x => x.Id == ExamHistory.ExamId);
                        if(Exam == null)
                        {
                            ExamHistory.AddError(nameof(AppUserValidator), nameof(ExamHistory.Exam), AppUserMessage.Error.ExamHistory_ExamNotExisted, AppUserMessage);
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
