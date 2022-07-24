using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam
{
    public class Exam_ImageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Guid RowId { get; set; }
        public Exam_ImageDTO() {}
        public Exam_ImageDTO(Image Image)
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

    public class Exam_ImageFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Url { get; set; }
        public string Search { get; set; }
        public ImageOrder OrderBy { get; set; }
    }
}