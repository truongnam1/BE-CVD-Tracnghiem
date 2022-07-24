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

namespace Tracnghiem.Services.MImage
{
    public interface IImageValidator : IServiceScoped
    {
        Task Get(Image Image);
        Task<bool> Create(Image Image);
        Task<bool> Update(Image Image);
        Task<bool> Delete(Image Image);
        Task<bool> BulkDelete(List<Image> Images);
        Task<bool> Import(List<Image> Images);
    }

    public class ImageValidator : IImageValidator
    {
        private IUOW UOW;
        private ICurrentContext CurrentContext;
        private ImageMessage ImageMessage;

        public ImageValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
            this.ImageMessage = new ImageMessage();
        }

        public async Task Get(Image Image)
        {
        }

        public async Task<bool> Create(Image Image)
        {
            await ValidateName(Image);
            await ValidateUrl(Image);
            return Image.IsValidated;
        }

        public async Task<bool> Update(Image Image)
        {
            if (await ValidateId(Image))
            {
                await ValidateName(Image);
                await ValidateUrl(Image);
            }
            return Image.IsValidated;
        }

        public async Task<bool> Delete(Image Image)
        {
            var oldData = await UOW.ImageRepository.Get(Image.Id);
            if (oldData != null)
            {
            }
            else
            {
                Image.AddError(nameof(ImageValidator), nameof(Image.Id), ImageMessage.Error.IdNotExisted, ImageMessage);
            }
            return Image.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Image> Images)
        {
            return Images.All(x => x.IsValidated);
        }

        public async Task<bool> Import(List<Image> Images)
        {
            return true;
        }
        
        private async Task<bool> ValidateId(Image Image)
        {
            ImageFilter ImageFilter = new ImageFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Image.Id },
                Selects = ImageSelect.Id
            };

            int count = await UOW.ImageRepository.CountAll(ImageFilter);
            if (count == 0)
                Image.AddError(nameof(ImageValidator), nameof(Image.Id), ImageMessage.Error.IdNotExisted, ImageMessage);
            return Image.IsValidated;
        }
        private async Task<bool> ValidateName(Image Image)
        {
            if (string.IsNullOrEmpty(Image.Name))
            {
                Image.AddError(nameof(ImageValidator), nameof(Image.Name), ImageMessage.Error.NameEmpty, ImageMessage);
            }
            else if (Image.Name.Count() > 500)
            {
                Image.AddError(nameof(ImageValidator), nameof(Image.Name), ImageMessage.Error.NameOverLength, ImageMessage);
            }
            return Image.IsValidated;
        }
        private async Task<bool> ValidateUrl(Image Image)
        {
            if (string.IsNullOrEmpty(Image.Url))
            {
                Image.AddError(nameof(ImageValidator), nameof(Image.Url), ImageMessage.Error.UrlEmpty, ImageMessage);
            }
            else if (Image.Url.Count() > 500)
            {
                Image.AddError(nameof(ImageValidator), nameof(Image.Url), ImageMessage.Error.UrlOverLength, ImageMessage);
            }
            return Image.IsValidated;
        }
    }
}
