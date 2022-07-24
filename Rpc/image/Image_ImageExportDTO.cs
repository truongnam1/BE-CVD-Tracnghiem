using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.image
{
    public class Image_ImageExportDTO : DataDTO
    {
        public long STT {get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Guid RowId { get; set; }
        public Image_ImageExportDTO() {}
        public Image_ImageExportDTO(Image Image)
        {
            this.Id = Image.Id;
            this.Name = Image.Name;
            this.Url = Image.Url;
            this.RowId = Image.RowId;
            this.Informations = Image.Informations;
            this.Warnings = Image.Warnings;
            this.Errors = Image.Errors;
        }
    }
}
