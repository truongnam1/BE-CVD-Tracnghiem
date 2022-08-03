using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Enums;
using Tracnghiem.Helpers;
using Tracnghiem.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Services.MSite
{
    public interface ISiteService : IServiceScoped
    {
        Task<int> Count(SiteFilter SiteFilter);
        Task<List<Site>> List(SiteFilter SiteFilter);
        Task<Site> Get(long Id);
        Task<Site> Update(Site Site);
        Task<Image> SaveImage(Image Image);
        Task<bool> BulkMerge(Site Site);
    }

    public class SiteService : BaseService, ISiteService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public SiteService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(SiteFilter SiteFilter)
        {
            try
            {
                int result = await UOW.SiteRepository.Count(SiteFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SiteService));
            }
            return 0;
        }

        public async Task<List<Site>> List(SiteFilter SiteFilter)
        {
            try
            {
                List<Site> Sites = await UOW.SiteRepository.List(SiteFilter);
                return Sites;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SiteService));
            }
            return null;
        }

        public async Task<Site> Get(long Id)
        {
            Site Site = await UOW.SiteRepository.Get(Id);
            if (Site == null)
                return null;
            return Site;
        }

        public async Task<Site> Update(Site Site)
        {
            try
            {
                var oldData = await UOW.SiteRepository.Get(Site.Id);


                Site.Code = oldData.Code;
                if (Site.Id == SiteEnum.LANDING.Id)
                    Site.IsDisplay = true;
                await UOW.SiteRepository.Update(Site);


                var newData = await UOW.SiteRepository.Get(Site.Id);
                return newData;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(SiteService));
            }
            return null;
        }

        public async Task<Image> SaveImage(Image Image)
        {
            //FileInfo fileInfo = new FileInfo(Image.Name);
            //string path = $"/site/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            //RestClient restClient = new RestClient($"{InternalServices.UTILS}");
            //RestRequest restRequest = new RestRequest("/rpc/utils/public-file/upload");
            //restRequest.RequestFormat = DataFormat.Json;
            //restRequest.Method = Method.POST;
            //restRequest.AddCookie("Token", CurrentContext.Token);
            //restRequest.AddCookie("X-Language", CurrentContext.Language);
            //restRequest.AddHeader("Content-Type", "multipart/form-data");
            //restRequest.AddFile("file", Image.Content, Image.Name);
            //restRequest.AddParameter("path", path);
            //try
            //{
            //    var response = await restClient.ExecuteAsync<Entities.File>(restRequest);
            //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //    {
            //        Image Result = new Image();
            //        Result.Id = response.Data.Id;
            //        Result.Url = "/rpc/utils/public-file/download" + response.Data.Path;
            //        Result.Name = Image.Name;
            //        return Result;
            //    }
            //}
            //catch
            //{
            //    return null;
            //}
            return null;
        }

        public async Task<bool> BulkMerge(Site Site)
        {
            try
            {
                SiteFilter SiteFilter = new SiteFilter
                {
                    Skip = 0,
                    Take = 1,
                    Code = new StringFilter { Equal = Site.Code },
                    Selects = SiteSelect.ALL,
                };
                List<Site> oldSites = await UOW.SiteRepository.List(SiteFilter);
                if (oldSites.Count == 0)
                {
                    Site NewSite = new Site
                    {
                        Code = Site.Code,
                        Name = Site.Name,
                        IsDisplay = false
                    };
                    await UOW.SiteRepository.Create(NewSite);
                    oldSites = await UOW.SiteRepository.List(SiteFilter);
                }

                Site.Menus.ForEach(x => x.SiteId = oldSites[0].Id);
                await UOW.MenuRepository.BulkMerge(Site);
                //await UOW.FieldEntityRepository.UpdateFieldName();
                var RoleAdminId = await UOW.RoleRepository.InitAdmin(Site.Code);
                var RoleUserId = await UOW.RoleRepository.InitUser(Site.Code);
                Site = await UOW.SiteRepository.Get(oldSites[0].Id);

                List<Role> Roles = await UOW.RoleRepository.List(new List<long> { RoleAdminId });

                return true;

            }
            catch (Exception ex)
            {

                Logging.CreateSystemLog(ex, nameof(SiteService));
            }
            return true;
        }
    }
}
