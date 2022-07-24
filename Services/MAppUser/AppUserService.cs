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

namespace Tracnghiem.Services.MAppUser
{
    public interface IAppUserService :  IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> BulkMerge(List<AppUser> AppUsers);
        Task<AppUserFilter> ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IAppUserValidator AppUserValidator;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IAppUserValidator AppUserValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.AppUserValidator = AppUserValidator;
        }

        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return 0;
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            await AppUserValidator.Get(AppUser);
            return AppUser;
        }
        
        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Create(AppUser);
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                Sync(new List<AppUser> { AppUser });
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);

                await UOW.AppUserRepository.Update(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                Sync(new List<AppUser> { AppUser });
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {
                await UOW.AppUserRepository.BulkDelete(AppUsers);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkMerge(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                var Ids = await UOW.AppUserRepository.BulkMerge(AppUsers);
                AppUsers = await UOW.AppUserRepository.List(Ids);
                Sync(AppUsers);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }     
        
        public async Task<AppUserFilter> ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "AppUserId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DisplayName))
                        subFilter.DisplayName = FilterBuilder.Merge(subFilter.DisplayName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RefreshToken))
                        subFilter.RefreshToken = FilterBuilder.Merge(subFilter.RefreshToken, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RoleId))
                        subFilter.RoleId = FilterBuilder.Merge(subFilter.RoleId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ImageId))
                        subFilter.ImageId = FilterBuilder.Merge(subFilter.ImageId, FilterPermissionDefinition.IdFilter);
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

        private void Sync(List<AppUser> AppUsers)
        {
            RabbitManager.PublishList(AppUsers, RoutingKeyEnum.AppUserSync.Code);


            
        }
    }
}
