using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracnghiem.Common;
using Tracnghiem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using System.Net;
using NGS.Templater;
using Tracnghiem.Entities;
using Tracnghiem.Services.MImage;

namespace Tracnghiem.Rpc.image
{
    public partial class ImageController : RpcController
    {
        private IImageService ImageService;
        private ICurrentContext CurrentContext;
        public ImageController(
            IImageService ImageService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ImageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            int count = await ImageService.Count(ImageFilter);
            return count;
        }

        [Route(ImageRoute.List), HttpPost]
        public async Task<ActionResult<List<Image_ImageDTO>>> List([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            List<Image> Images = await ImageService.List(ImageFilter);
            List<Image_ImageDTO> Image_ImageDTOs = Images
                .Select(c => new Image_ImageDTO(c)).ToList();
            return Image_ImageDTOs;
        }

        [Route(ImageRoute.Get), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Get([FromBody]Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = await ImageService.Get(Image_ImageDTO.Id);
            return new Image_ImageDTO(Image);
        }

        [Route(ImageRoute.Create), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Create(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            //if (!await HasPermission(Image_ImageDTO.Id))
            //    return Forbid();

            //Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image Image = await ImageService.Create(file);
            //Image_ImageDTO = new Image_ImageDTO(Image);
            //if (Image.IsValidated)
            //    return Image_ImageDTO;
            //else
            //    return BadRequest(Image_ImageDTO);

            return new Image_ImageDTO(Image);
        }
        private async Task<bool> HasPermission(long Id)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            if (Id == 0)
            {

            }
            else
            {
                ImageFilter.Id = new IdFilter { Equal = Id };
                int count = await ImageService.Count(ImageFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Image ConvertDTOToEntity(Image_ImageDTO Image_ImageDTO)
        {
            Image_ImageDTO.TrimString();
            Image Image = new Image();
            Image.Id = Image_ImageDTO.Id;
            Image.Name = Image_ImageDTO.Name;
            Image.Url = Image_ImageDTO.Url;
            Image.BaseLanguage = CurrentContext.Language;
            return Image;
        }

        private ImageFilter ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.SearchBy = ImageSearch.ALL;
            ImageFilter.Skip = Image_ImageFilterDTO.Skip;
            ImageFilter.Take = Image_ImageFilterDTO.Take;
            ImageFilter.OrderBy = Image_ImageFilterDTO.OrderBy;
            ImageFilter.OrderType = Image_ImageFilterDTO.OrderType;

            ImageFilter.Id = Image_ImageFilterDTO.Id;
            ImageFilter.Name = Image_ImageFilterDTO.Name;
            ImageFilter.Url = Image_ImageFilterDTO.Url;
            ImageFilter.Search = Image_ImageFilterDTO.Search;
            return ImageFilter;
        }
    }
}

