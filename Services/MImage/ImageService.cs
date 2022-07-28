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
using Microsoft.AspNetCore.Http;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tracnghiem.Services.MImage
{
    public interface IImageService :  IServiceScoped
    {
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<Image> Get(long Id);
        Task<Image> Create(IFormFile file);
        Task<ImageFilter> ToFilter(ImageFilter ImageFilter);
    }

    public class ImageService : BaseService, IImageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageValidator ImageValidator;

        private string APIKey = "ba4b149d644934850a218ea3aa6ede0b";

        public ImageService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IImageValidator ImageValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ImageValidator = ImageValidator;
        }

        public async Task<int> Count(ImageFilter ImageFilter)
        {
            try
            {
                int result = await UOW.ImageRepository.Count(ImageFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return 0;
        }

        public async Task<List<Image>> List(ImageFilter ImageFilter)
        {
            try
            {
                List<Image> Images = await UOW.ImageRepository.List(ImageFilter);
                return Images;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<Image> Get(long Id)
        {
            Image Image = await UOW.ImageRepository.Get(Id);
            if (Image == null)
                return null;
            await ImageValidator.Get(Image);
            return Image;
        }
        
        public async Task<Image> Create(IFormFile file)
        {
            //if (!await ImageValidator.Create(Image))
            //    return Image;

            if (file.Length < 0)
                return null;
                var ms = new MemoryStream();
                file.CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                //string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                
            string url = "/1/upload";
            // save original File
            RestClient restClient = new RestClient(ServiceEnum.IMGBB);
            RestRequest restRequest = new RestRequest(url);
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddQueryParameter("key", APIKey);
            restRequest.AddFile("image", fileBytes, file.FileName);

            //try
            //{
            //    await UOW.ImageRepository.Create(Image);
            //    Image = await UOW.ImageRepository.Get(Image.Id);
            //    return Image;
            //}
            //catch (Exception ex)
            //{
            //    Logging.CreateSystemLog(ex, nameof(ImageService));
            //}


            try
            {
                var response = restClient.Execute<string>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject json = JObject.Parse(response.Data);
                    Image Image = new Image();
                    Image.Id = 0;
                    Image.Name = json["data"]["title"].ToString();
                    Image.Url = json["data"]["url"].ToString();
                    Image.RowId = Guid.NewGuid();
                    Image = await UOW.ImageRepository.Create(Image);

                    return Image;
                }

                return new Image();
            }
            catch
            {

            }
            return null;
        }

        public async Task<ImageFilter> ToFilter(ImageFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ImageFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ImageFilter subFilter = new ImageFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "ImageId")
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Url))
                        subFilter.Url = FilterBuilder.Merge(subFilter.Url, FilterPermissionDefinition.StringFilter);
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
