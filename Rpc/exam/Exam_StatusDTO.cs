using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.exam
{
    public class Exam_StatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Exam_StatusDTO() {}
        public Exam_StatusDTO(Status Status)
        {
            this.Id = Status.Id;
            this.Code = Status.Code;
            this.Name = Status.Name;
            this.Informations = Status.Informations;
            this.Warnings = Status.Warnings;
            this.Errors = Status.Errors;
        }
    }

    public class Exam_StatusFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StatusOrder OrderBy { get; set; }
    }
}