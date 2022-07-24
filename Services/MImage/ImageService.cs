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

namespace Tracnghiem.Services.MImage
{
    public interface IImageService :  IServiceScoped
    {
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<Image> Get(long Id);
        Task<Image> Create(Image Image);
        Task<Image> Update(Image Image);
        Task<Image> Delete(Image Image);
        Task<List<Image>> BulkDelete(List<Image> Images);
        Task<List<Image>> BulkMerge(List<Image> Images);
        Task<ImageFilter> ToFilter(ImageFilter ImageFilter);
    }

    public class ImageService : BaseService, IImageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IImageValidator ImageValidator;

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
        
        public async Task<Image> Create(Image Image)
        {
            if (!await ImageValidator.Create(Image))
                return Image;

            try
            {
                await UOW.ImageRepository.Create(Image);
                Image = await UOW.ImageRepository.Get(Image.Id);
                return Image;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<Image> Update(Image Image)
        {
            if (!await ImageValidator.Update(Image))
                return Image;
            try
            {
                var oldData = await UOW.ImageRepository.Get(Image.Id);

                await UOW.ImageRepository.Update(Image);

                Image = await UOW.ImageRepository.Get(Image.Id);
                return Image;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<Image> Delete(Image Image)
        {
            if (!await ImageValidator.Delete(Image))
                return Image;

            try
            {
                await UOW.ImageRepository.Delete(Image);
                return Image;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<List<Image>> BulkDelete(List<Image> Images)
        {
            if (!await ImageValidator.BulkDelete(Images))
                return Images;

            try
            {
                await UOW.ImageRepository.BulkDelete(Images);
                return Images;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<List<Image>> BulkMerge(List<Image> Images)
        {
            if (!await ImageValidator.Import(Images))
                return Images;
            try
            {
                var Ids = await UOW.ImageRepository.BulkMerge(Images);
                Images = await UOW.ImageRepository.List(Ids);
                return Images;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
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
