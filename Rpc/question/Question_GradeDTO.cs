using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.question
{
    public class Question_GradeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Question_GradeDTO() {}
        public Question_GradeDTO(Grade Grade)
        {
            this.Id = Grade.Id;
            this.Code = Grade.Code;
            this.Name = Grade.Name;
            this.Informations = Grade.Informations;
            this.Warnings = Grade.Warnings;
            this.Errors = Grade.Errors;
        }
    }

    public class Question_GradeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public GradeOrder OrderBy { get; set; }
    }
}