using TrueSight.Common;
using Tracnghiem.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Tracnghiem.Entities;

namespace Tracnghiem.Rpc.public_exam
{
    public class PublicExam_ExamStatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public PublicExam_ExamStatusDTO() {}
        public PublicExam_ExamStatusDTO(ExamStatus ExamStatus)
        {
            this.Id = ExamStatus.Id;
            this.Code = ExamStatus.Code;
            this.Name = ExamStatus.Name;
            this.Informations = ExamStatus.Informations;
            this.Warnings = ExamStatus.Warnings;
            this.Errors = ExamStatus.Errors;
        }
    }

    public class PublicExam_ExamStatusFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public ExamStatusOrder OrderBy { get; set; }
    }
}